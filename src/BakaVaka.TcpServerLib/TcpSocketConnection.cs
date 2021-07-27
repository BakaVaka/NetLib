namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpSocketConnection<TMessage, TProtocol> :
        IConnection<TMessage, TProtocol>
        where TProtocol : IProtocol<TMessage>, new()
    {

        public event EventHandler Closed;

        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _closed;
        private EndPoint _localEndPoint;
        private EndPoint _remoteEndPoint;
        private NetworkStream _networkStream;
        private TProtocol _protocl = new();
        private readonly IMessageHandler<TMessage, TProtocol> _messageHandler;
        private Task _connectionRunTask;
        
        public TcpSocketConnection(Socket cleintSocket, IMessageHandler<TMessage, TProtocol> messageHandler)
        {
            
            _localEndPoint = cleintSocket.LocalEndPoint;
            _remoteEndPoint = cleintSocket.RemoteEndPoint;
            _networkStream = new NetworkStream(cleintSocket, true);
            _messageHandler = messageHandler;
        }

        public void Open()
        {
            Opened = DateTime.Now;
            _connectionRunTask ??= RunConnection();
        }

        private async Task RunConnection()
        {            
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var message = await Receive(_cancellationTokenSource.Token);
                    await _messageHandler.HandleMessage(message,this);
                }
            }
            catch (Exception) { }
            Close();
            
            async Task<TMessage> Receive(CancellationToken cancellationToken)
            {
                ThrowIfDisposed();
                var message = await _protocl.Decode(_networkStream, this, cancellationToken);
                LastAcceptedMessageDateTime = DateTime.Now;
                return message;
            }

        }

        public virtual void OnIdle() { }
        public EndPoint LocalEndPoint => _localEndPoint;

        public EndPoint RemoteEndPoint => _remoteEndPoint;

        public DateTime LastAcceptedMessageDateTime { get; private set; }

        public DateTime LastSentMessageDateTime { get; private set; }
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Opened { get; private set;  }

        public virtual async Task Send(TMessage message)
        {
            ThrowIfDisposed();
            if (message is TMessage protocolMessage)
            {
                var data = _protocl.Encode(protocolMessage, this);
                await _networkStream.WriteAsync(data, _cancellationTokenSource.Token);
                LastSentMessageDateTime = DateTime.Now;
            }
        }


        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException($"Tcp connection {Id}");
            }
        }
        ~TcpSocketConnection() => Dispose(false);
        public void Dispose() => Dispose(true);

        private void Dispose(bool isDispose)
        {
            if (!_disposed)
            {
                _disposed = true;
                _networkStream.Dispose();
                if (isDispose)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        public void Close()
        {
            if (!_closed)
            {
                _closed = true;
                _cancellationTokenSource.Cancel();
                _networkStream.Close();
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}