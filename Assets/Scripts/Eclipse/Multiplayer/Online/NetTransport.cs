using System;
using System.Net;
using System.Net.Sockets;

namespace Eclipse.Multiplayer.Online
{
    /// <summary>Where a <see cref="NetplayPeer"/> sends and receives its datagrams.</summary>
    public interface INetTransport : IDisposable
    {
        /// <summary>Delivers every pending datagram to <paramref name="handler"/> (sender, buffer, length).</summary>
        void Poll(Action<EndPoint, byte[], int> handler);
        void Send(EndPoint to, byte[] data, int length);
        int LocalPort { get; }
    }

    /// <summary>A plain non-blocking UDP socket.</summary>
    public sealed class UdpTransport : INetTransport
    {
        private readonly System.Net.Sockets.Socket _socket;
        private readonly byte[] _buffer = new byte[2048];
        private bool _disposed;

        public UdpTransport(int port)
        {
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };
            DisableConnectionReset(_socket);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public int LocalPort => ((IPEndPoint)_socket.LocalEndPoint).Port;

        public void Poll(Action<EndPoint, byte[], int> handler)
        {
            for (int guard = 0; guard < 512 && !_disposed; guard++)
            {
                EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                int length;
                try
                {
                    if (_socket.Available <= 0) return;
                    length = _socket.ReceiveFrom(_buffer, ref from);
                }
                catch (SocketException exception) when (exception.SocketErrorCode == SocketError.WouldBlock ||
                    exception.SocketErrorCode == SocketError.ConnectionReset || exception.SocketErrorCode == SocketError.MessageSize)
                {
                    continue;
                }
                catch (ObjectDisposedException) { return; }
                try { handler(from, _buffer, length); }
                catch (NetFormatException) { /* Malformed datagram; drop it. */ }
            }
        }

        public void Send(EndPoint to, byte[] data, int length)
        {
            if (to == null || _disposed) return;
            try { _socket.SendTo(data, 0, length, SocketFlags.None, to); }
            catch (SocketException) { /* Transient network failures surface as timeouts. */ }
            catch (ObjectDisposedException) { }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _socket.Close();
        }

        private static void DisableConnectionReset(System.Net.Sockets.Socket socket)
        {
            // Windows reports ICMP port-unreachable as ConnectionReset on later receives.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return;
            try
            {
                const int SioUdpConnReset = -1744830452;
                socket.IOControl(SioUdpConnReset, new byte[] { 0 }, null);
            }
            catch (Exception) { }
        }
    }
}
