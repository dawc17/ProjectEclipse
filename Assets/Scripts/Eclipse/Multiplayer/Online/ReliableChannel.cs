using System;
using System.Collections.Generic;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// Ordered, reliable messages over the unreliable data stream. Every outgoing data
    /// packet carries a cumulative ack and resends whatever the peer has not acked yet.
    /// Sequence numbers are 16-bit and compared with wrap-around.
    /// </summary>
    public sealed class ReliableChannel
    {
        public const int MaxMessageSize = 400;
        public const int MaxPending = 256;
        public const int ResendMs = 120;

        private sealed class Outgoing
        {
            public ushort Seq;
            public byte[] Payload;
            public long LastSentMs = long.MinValue;
        }

        private readonly List<Outgoing> _outgoing = new List<Outgoing>();
        private readonly Dictionary<ushort, byte[]> _early = new Dictionary<ushort, byte[]>();
        private readonly Queue<byte[]> _delivered = new Queue<byte[]>();
        private ushort _nextSendSeq;
        private ushort _nextReceiveSeq;

        public int PendingCount => _outgoing.Count;
        public int DeliveredCount => _delivered.Count;

        public void Send(byte[] payload)
        {
            if (payload == null || payload.Length == 0) throw new ArgumentException("Empty reliable message.", nameof(payload));
            if (payload.Length > MaxMessageSize) throw new ArgumentException("Reliable message is too large.", nameof(payload));
            if (_outgoing.Count >= MaxPending) throw new InvalidOperationException("Too many unacknowledged messages.");
            _outgoing.Add(new Outgoing { Seq = _nextSendSeq++, Payload = payload });
        }

        public bool TryReceive(out byte[] payload)
        {
            if (_delivered.Count == 0) { payload = null; return false; }
            payload = _delivered.Dequeue();
            return true;
        }

        /// <summary>Writes the ack and every message that is due, leaving <paramref name="reserve"/> bytes.</summary>
        public void Write(NetWriter writer, long nowMs, int reserve)
        {
            writer.U16(_nextReceiveSeq);
            int countPosition = writer.Length;
            writer.U8(0);
            int written = 0;
            foreach (var message in _outgoing)
            {
                if (written == byte.MaxValue) break;
                if (message.LastSentMs != long.MinValue && nowMs - message.LastSentMs < ResendMs) continue;
                if (writer.Remaining - reserve < message.Payload.Length + 4) break;
                writer.U16(message.Seq);
                writer.U16((ushort)message.Payload.Length);
                writer.Bytes(message.Payload, 0, message.Payload.Length);
                message.LastSentMs = nowMs;
                written++;
            }
            writer.Buffer[countPosition] = (byte)written;
        }

        public void Read(NetReader reader)
        {
            Acknowledge(reader.U16());
            int count = reader.U8();
            for (int i = 0; i < count; i++)
            {
                ushort seq = reader.U16();
                int length = reader.U16();
                if (length == 0 || length > MaxMessageSize) throw new NetFormatException("Bad reliable message length.");
                byte[] payload = reader.Bytes(length);
                Accept(seq, payload);
            }
        }

        private void Acknowledge(ushort nextExpectedByPeer)
        {
            // Everything strictly before the peer's next expected sequence has arrived.
            _outgoing.RemoveAll(message => SeqLess(message.Seq, nextExpectedByPeer));
        }

        private void Accept(ushort seq, byte[] payload)
        {
            if (SeqLess(seq, _nextReceiveSeq)) return; // duplicate
            if (seq != _nextReceiveSeq)
            {
                if (_early.Count < MaxPending) _early[seq] = payload;
                return;
            }
            _delivered.Enqueue(payload);
            _nextReceiveSeq++;
            while (_early.TryGetValue(_nextReceiveSeq, out var next))
            {
                _early.Remove(_nextReceiveSeq);
                _delivered.Enqueue(next);
                _nextReceiveSeq++;
            }
        }

        public static bool SeqLess(ushort a, ushort b) => (short)(a - b) < 0;
    }
}
