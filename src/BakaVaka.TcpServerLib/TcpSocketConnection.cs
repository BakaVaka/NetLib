using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public class TcpSocketConnection<TMessage, TProtocl, TContext> : IConnection<TMessage, TProtocl, TContext>
        where TProtocl : IProtocol<TMessage, TContext>, new()
        where TContext : IConnectionContext, new()
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _closed;
        private EndPoint _localEndPoint;
        private EndPoint _remoteEndPoint;

        private readonly NetworkStream _networkStream;
        private TProtocl _protocl = new();
        private TContext _context = new();

        public TcpSocketConnection(Socket socket)
        {
            _localEndPoint = socket.LocalEndPoint;
            _remoteEndPoint = socket.RemoteEndPoint;
            _networkStream = new NetworkStream(socket, true);
            _context.Bind(this);
        }

        public TContext Contex => _context;

        public EndPoint LocalEndPoint => _localEndPoint;

        public EndPoint RemoteEndPoint => _remoteEndPoint;

        public DateTime LastRecievedDateTime { get; private set; }

        public DateTime LastSentDateTime { get; private set; }

        public string ID { get; } = Guid.NewGuid().ToString();


        public async Task<TMessage> Receive(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var message = await _protocl.Decode(_networkStream, Contex);
            LastRecievedDateTime = DateTime.Now;
            return message;
        }

        public async Task Send<T>(T message)
        {
            ThrowIfDisposed();
            if(message is TMessage protocolMessage)
            {
                var data = _protocl.Encode(protocolMessage, Contex);
                await _networkStream.WriteAsync(data, _cancellationTokenSource.Token);
                LastSentDateTime = DateTime.Now;
            }
        }


        private bool _disposed;

        public event EventHandler Closed;

        private void ThrowIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException($"Tcp connection {ID}");
            }
        }
        ~TcpSocketConnection() => Dispose(false);
        public void Dispose() => Dispose(true);

        private void Dispose(bool isDispose)
        {
            if(!_disposed)
            {
                _disposed = true;
                //TODO освобождать ресурсы
                _cancellationTokenSource.Dispose();
                _networkStream.Dispose();
                _context.Dispose();
                if(isDispose)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Close()
        {
            if(!_closed)
            {
                _closed = true;

                _cancellationTokenSource.Cancel();
                _networkStream.Close();

            }
        }
    }
}