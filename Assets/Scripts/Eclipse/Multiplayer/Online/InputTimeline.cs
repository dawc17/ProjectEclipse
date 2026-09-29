using System;
using System.Collections.Generic;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// Input history for one online match. Local input sampled while simulating tick T
    /// is scheduled for tick T + Delay; the first Delay ticks are neutral on both sides.
    /// <para>
    /// With <see cref="MaxPrediction"/> 0 this is delay-based lockstep: a tick may only
    /// be simulated once both peers' inputs for it are known. With rollback, a tick may
    /// run up to MaxPrediction ticks past the opponent's last confirmed input, predicting
    /// that they keep holding it. When the real input arrives and differs from the guess,
    /// <see cref="PendingRollback"/> names the first wrong tick so the caller can restore
    /// the state saved before it and simulate forward again.
    /// </para>
    /// State hashes are only compared for final ticks: ticks simulated with confirmed
    /// inputs and not waiting to be re-simulated.
    /// </summary>
    public sealed class InputTimeline
    {
        public const int MaxInputsPerPacket = 240;
        public const int MaxRunsPerPacket = 120;
        /// <summary>Upper bound on the prediction window either peer may ask for.</summary>
        public const int MaxPredictionLimit = 12;
        private const int HashHistory = 600;
        private const int PredictionRing = 64;
        private const float AdvantageSmoothing = 0.1f;

        private readonly List<byte> _local = new List<byte>();
        private readonly List<byte> _remote = new List<byte>();
        private readonly Dictionary<int, uint> _localHashes = new Dictionary<int, uint>();
        private readonly Dictionary<int, uint> _remoteHashes = new Dictionary<int, uint>();
        private readonly byte[] _usedRemote = new byte[PredictionRing];
        private readonly bool[] _usedPrediction = new bool[PredictionRing];
        private int _peerHasLocal;
        private int _simulated;
        // Ticks below this hold results from the current inputs; a re-simulation
        // walks it back up to _simulated.
        private int _validThrough;
        private int _comparedThrough = -1;

        // Time sync: the peer's reported tick, when it arrived, and its view of the gap.
        private int _remoteTick = -1;
        private long _remoteTickReceivedMs;
        private float _remoteAdvantage;
        private bool _hasAdvantage;

        public int MatchIndex { get; }
        public int Delay { get; }
        /// <summary>How many ticks may run on a predicted remote input; 0 is lockstep.</summary>
        public int MaxPrediction { get; }
        public bool IsRollback => MaxPrediction > 0;
        public int LocalFrames => _local.Count;
        /// <summary>Remote inputs confirmed so far (ticks 0 .. RemoteFrames - 1).</summary>
        public int RemoteFrames => _remote.Count;
        /// <summary>Ticks simulated so far, including ones that ran on a prediction.</summary>
        public int SimulatedTicks => _simulated;
        /// <summary>Earliest tick simulated on a wrong prediction, or -1.</summary>
        public int PendingRollback { get; private set; } = -1;
        /// <summary>Earliest tick whose state hashes disagreed, or -1.</summary>
        public int DesyncTick { get; private set; } = -1;
        public int VerifiedTick { get; private set; } = -1;
        /// <summary>Smoothed ticks this peer runs ahead of the opponent (negative when behind).</summary>
        public float FrameAdvantage { get; private set; }
        public int Rollbacks { get; private set; }
        public int RolledBackTicks { get; private set; }
        public int LongestRollback { get; private set; }

        /// <summary>Ticks below this ran on confirmed inputs and will never be simulated again.</summary>
        public int FinalTicks
        {
            get
            {
                int final = Math.Min(_remote.Count, _validThrough);
                return PendingRollback >= 0 ? Math.Min(final, PendingRollback) : final;
            }
        }

        public InputTimeline(int matchIndex, int delay, int maxPrediction = 0)
        {
            if (delay < 0 || delay > NetProtocol.MaxInputDelay) throw new ArgumentOutOfRangeException(nameof(delay));
            if (maxPrediction < 0 || maxPrediction > MaxPredictionLimit) throw new ArgumentOutOfRangeException(nameof(maxPrediction));
            MatchIndex = matchIndex & 0xFF;
            Delay = delay;
            MaxPrediction = maxPrediction;
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

        /// <summary>Both confirmed inputs for <paramref name="tick"/> (lockstep).</summary>
        public bool TryGetInputs(int tick, out byte local, out byte remote)
        {
            if (tick < 0 || tick >= _local.Count || tick >= _remote.Count) { local = remote = 0; return false; }
            local = _local[tick];
            remote = _remote[tick];
            return true;
        }

        /// <summary>
        /// Inputs for <paramref name="tick"/>, predicting the remote one when it has not
        /// arrived and the prediction window allows. False means the tick must wait.
        /// </summary>
        public bool TryGetInputs(int tick, out byte local, out byte remote, out bool predicted)
        {
            predicted = false;
            if (tick < 0 || tick >= _local.Count) { local = remote = 0; return false; }
            local = _local[tick];
            if (tick < _remote.Count) { remote = _remote[tick]; return true; }
            if (tick >= _remote.Count + MaxPrediction) { remote = 0; return false; }
            remote = PredictRemote();
            predicted = true;
            return true;
        }

        /// <summary>The opponent keeps holding whatever they held last.</summary>
        public byte PredictRemote() => _remote.Count > 0 ? _remote[_remote.Count - 1] : NetInput.Neutral;

        /// <summary>Records that <paramref name="tick"/> ran with <paramref name="remote"/> as the opponent's input.</summary>
        public void MarkSimulated(int tick, byte remote, bool predicted)
        {
            if (tick != _validThrough) throw new InvalidOperationException("Ticks must be simulated in order (expected " + _validThrough + ", got " + tick + ").");
            _usedRemote[tick & (PredictionRing - 1)] = remote;
            _usedPrediction[tick & (PredictionRing - 1)] = predicted && tick >= _remote.Count;
            _validThrough = tick + 1;
            if (_validThrough > _simulated) _simulated = _validThrough;
            // A hash from an earlier run of this tick is stale; RecordLocalHash supplies the new one.
            _localHashes.Remove(tick);
            CompareFinal();
        }

        /// <summary>True while ticks restored by a rollback still have to be simulated again.</summary>
        public bool IsResimulating => _validThrough < _simulated;

        /// <summary>
        /// The caller restored the state from before <paramref name="tick"/> (at or before
        /// <see cref="PendingRollback"/>) and will simulate every tick up to
        /// <see cref="SimulatedTicks"/> again.
        /// </summary>
        public void BeginResimulation(int tick)
        {
            if (tick < 0 || tick > _validThrough) throw new ArgumentOutOfRangeException(nameof(tick));
            if (PendingRollback >= 0 && tick > PendingRollback) throw new ArgumentOutOfRangeException(nameof(tick), "Restore at or before the pending rollback.");
            int depth = _simulated - tick;
            Rollbacks++;
            RolledBackTicks += depth;
            if (depth > LongestRollback) LongestRollback = depth;
            _validThrough = tick;
            PendingRollback = -1;
        }

        /// <summary>
        /// Drops simulated ticks from <paramref name="tick"/> on: the caller restored the
        /// state before it and simulates forward from there as new ticks.
        /// </summary>
        public void Rewind(int tick)
        {
            if (tick < 0 || tick > _validThrough) throw new ArgumentOutOfRangeException(nameof(tick));
            _simulated = _validThrough = tick;
            if (PendingRollback >= tick) PendingRollback = -1;
        }

        public byte GetLocal(int tick) => _local[tick];
        public byte GetRemote(int tick) => _remote[tick];

        /// <summary>Records this peer's state hash after simulating <paramref name="tick"/> (overwritten on re-simulation).</summary>
        public void RecordLocalHash(int tick, uint hash)
        {
            _localHashes[tick] = hash;
            _localHashes.Remove(tick - HashHistory);
            if (tick < FinalTicks) Compare(tick);
            CompareFinal();
        }

        public bool TryGetLocalHash(int tick, out uint hash) => _localHashes.TryGetValue(tick, out hash);

        /// <summary>Writes unacknowledged local inputs (run-length coded), time sync and the latest final state hash.</summary>
        public void WriteSync(NetWriter writer, long nowMs = 0, int rttMs = -1)
        {
            writer.U8((byte)MatchIndex);
            writer.I32(_remote.Count);
            writer.I32(_simulated);
            writer.U16(unchecked((ushort)(short)Math.Max(short.MinValue, Math.Min(short.MaxValue, Math.Round(LocalAdvantage(nowMs, rttMs) * 16)))));
            int start = _peerHasLocal;
            writer.I32(start);
            int runsAt = writer.Length;
            writer.U8(0);
            int runs = 0, covered = 0, available = Math.Min(_local.Count - start, MaxInputsPerPacket);
            while (covered < available && runs < MaxRunsPerPacket && writer.Remaining >= 2 + 8)
            {
                byte value = _local[start + covered];
                int length = 1;
                while (covered + length < available && length < 255 && _local[start + covered + length] == value) length++;
                writer.U8(value);
                writer.U8((byte)length);
                covered += length;
                runs++;
            }
            writer.Buffer[runsAt] = (byte)runs;
            int hashTick = LatestFinalHashTick();
            writer.I32(hashTick);
            writer.U32(hashTick >= 0 ? _localHashes[hashTick] : 0u);
        }

        /// <returns>False when the block belongs to a different match and was ignored.</returns>
        public bool ReadSync(NetReader reader, long nowMs = 0, int rttMs = -1)
        {
            int matchIndex = reader.U8();
            if (matchIndex != MatchIndex) return false;
            int peerHas = reader.I32();
            int remoteTick = reader.I32();
            float remoteAdvantage = unchecked((short)reader.U16()) / 16f;
            if (peerHas > _peerHasLocal && peerHas <= _local.Count) _peerHasLocal = peerHas;
            int start = reader.I32();
            int runs = reader.U8();
            if (runs > MaxRunsPerPacket || start < 0) throw new NetFormatException("Bad input range.");
            int position = start;
            for (int run = 0; run < runs; run++)
            {
                byte input = reader.U8();
                int length = reader.U8();
                if (!NetInput.IsValid(input)) throw new NetFormatException("Invalid input byte.");
                if (length == 0 || position - start + length > MaxInputsPerPacket) throw new NetFormatException("Bad input run.");
                for (int i = 0; i < length; i++, position++)
                    if (position == _remote.Count) ConfirmRemote(input);
            }
            int hashTick = reader.I32();
            uint hash = reader.U32();
            if (hashTick >= 0 && !_remoteHashes.ContainsKey(hashTick))
            {
                _remoteHashes[hashTick] = hash;
                PruneRemoteHashes(hashTick - HashHistory);
                if (hashTick < FinalTicks) Compare(hashTick);
            }
            CompareFinal();
            if (remoteTick >= _remoteTick)
            {
                _remoteTick = remoteTick;
                _remoteTickReceivedMs = nowMs;
                _remoteAdvantage = remoteAdvantage;
                // Averaging both peers' views cancels most of the one-way latency estimate's error.
                float sample = (LocalAdvantage(nowMs, rttMs) - remoteAdvantage) / 2f;
                FrameAdvantage = _hasAdvantage ? FrameAdvantage + (sample - FrameAdvantage) * AdvantageSmoothing : sample;
                _hasAdvantage = true;
            }
            return true;
        }

        // Final ticks advance in bursts, so old entries are swept rather than removed by exact key.
        private readonly List<int> _pruned = new List<int>();
        private void PruneRemoteHashes(int below)
        {
            if (_remoteHashes.Count < HashHistory) return;
            _pruned.Clear();
            foreach (int tick in _remoteHashes.Keys) if (tick < below) _pruned.Add(tick);
            foreach (int tick in _pruned) _remoteHashes.Remove(tick);
        }

        /// <summary>The caller waited one tick to let the opponent catch up.</summary>
        public void ConsumeAdvantage(float ticks) => FrameAdvantage -= ticks;

        private void ConfirmRemote(byte input)
        {
            int tick = _remote.Count;
            _remote.Add(input);
            int slot = tick & (PredictionRing - 1);
            // Only results already computed can be wrong; ticks a re-simulation has not
            // reached yet will read the confirmed input when it gets there.
            if (tick < _validThrough && _usedPrediction[slot])
            {
                _usedPrediction[slot] = false;
                if (_usedRemote[slot] != input && (PendingRollback < 0 || tick < PendingRollback)) PendingRollback = tick;
            }
        }

        /// <summary>How far this peer's tick is ahead of its estimate of the opponent's current tick.</summary>
        private float LocalAdvantage(long nowMs, int rttMs)
        {
            if (_remoteTick < 0) return 0f;
            float oneWayMs = rttMs > 0 ? rttMs / 2f : 0f;
            float remoteNow = _remoteTick + (Math.Max(0, nowMs - _remoteTickReceivedMs) + oneWayMs) / NetProtocol.TickMs;
            return _simulated - remoteNow;
        }

        private int LatestFinalHashTick()
        {
            int final = FinalTicks;
            for (int tick = final - 1; tick >= 0 && tick >= final - 2 * NetProtocol.HashInterval; tick--)
                if (_localHashes.ContainsKey(tick)) return tick;
            return -1;
        }

        private void CompareFinal()
        {
            int final = FinalTicks;
            for (int tick = Math.Max(_comparedThrough + 1, final - HashHistory); tick < final; tick++) Compare(tick);
            if (final - 1 > _comparedThrough) _comparedThrough = final - 1;
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
