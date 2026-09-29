using System;
using System.Collections.Generic;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// Delay-based lockstep input history for one match. Local input sampled while
    /// simulating tick T is scheduled for tick T + Delay; a tick may only be simulated
    /// once both peers' inputs for it are known. The first Delay ticks are neutral.
    /// </summary>
    public sealed class LockstepTimeline
    {
        public const int MaxInputsPerPacket = 120;
        private const int HashHistory = 256;

        private readonly List<byte> _local = new List<byte>();
        private readonly List<byte> _remote = new List<byte>();
        private readonly Dictionary<int, uint> _localHashes = new Dictionary<int, uint>();
        private readonly Dictionary<int, uint> _remoteHashes = new Dictionary<int, uint>();
        private int _peerHasLocal;
        private int _latestLocalHashTick = -1;

        public int MatchIndex { get; }
        public int Delay { get; }
        public int LocalFrames => _local.Count;
        public int RemoteFrames => _remote.Count;
        /// <summary>Earliest tick whose state hashes disagreed, or -1.</summary>
        public int DesyncTick { get; private set; } = -1;
        public int VerifiedTick { get; private set; } = -1;

        public LockstepTimeline(int matchIndex, int delay)
        {
            if (delay < 0 || delay > NetProtocol.MaxInputDelay) throw new ArgumentOutOfRangeException(nameof(delay));
            MatchIndex = matchIndex & 0xFF;
            Delay = delay;
            for (int i = 0; i < delay; i++)
            {
                _local.Add(NetInput.Neutral);
                _remote.Add(NetInput.Neutral);
            }
            _peerHasLocal = delay;
        }

        /// <summary>True when the local input for <paramref name="tick"/> + Delay is still unsampled.</summary>
        public bool NeedsLocalInput(int tick) => _local.Count == tick + Delay;

        public void AddLocal(byte input)
        {
            if (!NetInput.IsValid(input)) throw new ArgumentException("Invalid input.", nameof(input));
            _local.Add(input);
        }

        public bool TryGetInputs(int tick, out byte local, out byte remote)
        {
            if (tick < 0 || tick >= _local.Count || tick >= _remote.Count) { local = remote = 0; return false; }
            local = _local[tick];
            remote = _remote[tick];
            return true;
        }

        public byte GetLocal(int tick) => _local[tick];
        public byte GetRemote(int tick) => _remote[tick];

        public void RecordLocalHash(int tick, uint hash)
        {
            _localHashes[tick] = hash;
            _latestLocalHashTick = tick;
            Compare(tick);
            _localHashes.Remove(tick - HashHistory * NetProtocol.HashInterval);
        }

        /// <summary>Writes unacknowledged local inputs and the latest local state hash.</summary>
        public void WriteSync(NetWriter writer)
        {
            writer.U8((byte)MatchIndex);
            writer.I32(_remote.Count);
            int start = _peerHasLocal;
            int count = Math.Min(Math.Min(_local.Count - start, MaxInputsPerPacket), writer.Remaining - 16);
            if (count < 0) count = 0;
            writer.I32(start);
            writer.U8((byte)count);
            for (int i = 0; i < count; i++) writer.U8(_local[start + i]);
            writer.I32(_latestLocalHashTick);
            writer.U32(_latestLocalHashTick >= 0 ? _localHashes[_latestLocalHashTick] : 0u);
        }

        /// <returns>False when the block belongs to a different match and was ignored.</returns>
        public bool ReadSync(NetReader reader)
        {
            int matchIndex = reader.U8();
            if (matchIndex != MatchIndex) return false;
            int peerHas = reader.I32();
            if (peerHas > _peerHasLocal && peerHas <= _local.Count) _peerHasLocal = peerHas;
            int start = reader.I32();
            int count = reader.U8();
            if (count > MaxInputsPerPacket || start < 0) throw new NetFormatException("Bad input range.");
            for (int i = 0; i < count; i++)
            {
                byte input = reader.U8();
                if (!NetInput.IsValid(input)) throw new NetFormatException("Invalid input byte.");
                if (start + i == _remote.Count) _remote.Add(input);
            }
            int hashTick = reader.I32();
            uint hash = reader.U32();
            if (hashTick >= 0 && !_remoteHashes.ContainsKey(hashTick))
            {
                _remoteHashes[hashTick] = hash;
                _remoteHashes.Remove(hashTick - HashHistory * NetProtocol.HashInterval);
                Compare(hashTick);
            }
            return true;
        }

        private void Compare(int tick)
        {
            if (!_localHashes.TryGetValue(tick, out var mine) || !_remoteHashes.TryGetValue(tick, out var theirs)) return;
            if (mine == theirs)
            {
                if (tick > VerifiedTick) VerifiedTick = tick;
            }
            else if (DesyncTick < 0 || tick < DesyncTick)
            {
                DesyncTick = tick;
            }
        }
    }
}
