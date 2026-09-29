using System;
using System.Text;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>Little-endian packet writer with a fixed capacity.</summary>
    public sealed class NetWriter
    {
        private readonly byte[] _buffer;
        public int Length { get; private set; }
        public byte[] Buffer => _buffer;

        public NetWriter(int capacity = NetProtocol.MaxPacketSize) { _buffer = new byte[capacity]; }

        public void Reset() { Length = 0; }

        public void U8(byte value) { Ensure(1); _buffer[Length++] = value; }
        public void Bool(bool value) => U8(value ? (byte)1 : (byte)0);

        public void U16(ushort value)
        {
            Ensure(2);
            _buffer[Length++] = (byte)value;
            _buffer[Length++] = (byte)(value >> 8);
        }

        public void U32(uint value)
        {
            Ensure(4);
            _buffer[Length++] = (byte)value;
            _buffer[Length++] = (byte)(value >> 8);
            _buffer[Length++] = (byte)(value >> 16);
            _buffer[Length++] = (byte)(value >> 24);
        }

        public void I32(int value) => U32(unchecked((uint)value));

        public void Bytes(byte[] source, int offset, int count)
        {
            if (count < 0 || offset < 0 || source == null || offset + count > source.Length) throw new ArgumentOutOfRangeException(nameof(count));
            Ensure(count);
            Array.Copy(source, offset, _buffer, Length, count);
            Length += count;
        }

        public void Str(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            if (bytes.Length > NetProtocol.MaxStringBytes) throw new ArgumentException("String is too long for a packet.");
            U8((byte)bytes.Length);
            Bytes(bytes, 0, bytes.Length);
        }

        public int Remaining => _buffer.Length - Length;

        public byte[] ToArray()
        {
            var copy = new byte[Length];
            Array.Copy(_buffer, copy, Length);
            return copy;
        }

        private void Ensure(int count)
        {
            if (Length + count > _buffer.Length) throw new InvalidOperationException("Packet capacity exceeded.");
        }
    }

    /// <summary>Bounds-checked reader. Any malformed read throws <see cref="NetFormatException"/>.</summary>
    public sealed class NetReader
    {
        private readonly byte[] _buffer;
        private readonly int _end;
        public int Position { get; private set; }
        public int Remaining => _end - Position;

        public NetReader(byte[] buffer, int offset, int count)
        {
            if (buffer == null || offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));
            _buffer = buffer;
            Position = offset;
            _end = offset + count;
        }

        public NetReader(byte[] buffer) : this(buffer, 0, buffer?.Length ?? 0) { }

        public byte U8() { Need(1); return _buffer[Position++]; }
        public bool Bool() => U8() != 0;

        public ushort U16()
        {
            Need(2);
            int value = _buffer[Position] | (_buffer[Position + 1] << 8);
            Position += 2;
            return (ushort)value;
        }

        public uint U32()
        {
            Need(4);
            uint value = (uint)(_buffer[Position] | (_buffer[Position + 1] << 8) | (_buffer[Position + 2] << 16) | (_buffer[Position + 3] << 24));
            Position += 4;
            return value;
        }

        public int I32() => unchecked((int)U32());

        public byte[] Bytes(int count)
        {
            if (count < 0) throw new NetFormatException("Negative length.");
            Need(count);
            var result = new byte[count];
            Array.Copy(_buffer, Position, result, 0, count);
            Position += count;
            return result;
        }

        public void CopyTo(byte[] target, int offset, int count)
        {
            Need(count);
            Array.Copy(_buffer, Position, target, offset, count);
            Position += count;
        }

        public string Str()
        {
            int length = U8();
            Need(length);
            string value = Encoding.UTF8.GetString(_buffer, Position, length);
            Position += length;
            return value;
        }

        private void Need(int count)
        {
            if (count > Remaining) throw new NetFormatException("Packet ended early.");
        }
    }

    public sealed class NetFormatException : Exception
    {
        public NetFormatException(string message) : base(message) { }
    }
}
