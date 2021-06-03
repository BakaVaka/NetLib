using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public class TcpSocketConnection : ITransportConnection
    {
        private readonly Socket _socket;
        private readonly TcpServer _server;

        public TcpSocketConnection(Socket socket, TcpServer server)
        {
            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }
            _socket = socket;
            _server = server;
            LocalEndPoint = _socket.LocalEndPoint;
            RemoteEndPoint = _socket.RemoteEndPoint;
            _server.Stopped += OnServerStopped;
        }

        public EndPoint LocalEndPoint { get; }

        public EndPoint RemoteEndPoint { get; }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        public ValueTask<int> Receve(Memory<byte> buffer)
        {
            return _socket.ReceiveAsync(buffer, SocketFlags.None);
        }
        public ValueTask<int> Send(ReadOnlyMemory<byte> message)
        {
            return _socket.SendAsync(message, SocketFlags.None);
        }
        private void OnServerStopped()
        {
            _server.Stopped -= OnServerStopped;
            Dispose();
        }
    }
}