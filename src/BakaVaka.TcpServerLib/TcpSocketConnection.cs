namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpSocketConnection<TMessage, TProtocol, TContext> : 
        IConnection<TMessage, TProtocol, TContext>
        where TProtocol : IProtocol<TMessage, TContext>, new()
        where TContext : IConnectionContext, new()
    {

        public event EventHandler Closed;

        private Func<TMessage, Task> _messageCallback;

        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _closed;
        private EndPoint _localEndPoint;
        private EndPoint _remoteEndPoint;

        private NetworkStream _networkStream;
        private TProtocol _protocl = new();
        private TContext _context = new();

        private readonly object _bindLocker = new();
        private bool _binded;
        private Task _connectionRunTask;

        public void BindSocket(Socket socket)
        {
            lock(_bindLocker)
            {
                if(_binded)
                {
                    throw new InvalidOperationException("Connection already binded to socket");
                }
                _localEndPoint = socket.LocalEndPoint;
                _remoteEndPoint = socket.RemoteEndPoint;
                _networkStream = new NetworkStream(socket, true);
                _context.Bind(this);
            }
            _connectionRunTask = RunConnection();
        }

        private async Task RunConnection()
        {
            try
            {
                while(!_cancellationTokenSource.IsCancellationRequested)
                {

                    var message = await Receive(_cancellationTokenSource.Token);
                    await _messageCallback.Invoke(message);
                }
            }
            catch(Exception) { }
            Close();
        }

        public virtual void OnIdle(){}

        public TContext Contex => _context;

        public EndPoint LocalEndPoint => _localEndPoint;

        public EndPoint RemoteEndPoint => _remoteEndPoint;

        public DateTime LastAcceptedMessageDateTime { get; private set; }

        public DateTime LastSentMessageDateTime { get; private set; }
        public Guid Id { get; } = Guid.NewGuid();


        private async Task<TMessage> Receive(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            var message = await _protocl.Decode(_networkStream, Contex, cancellationToken);
            LastAcceptedMessageDateTime = DateTime.Now;
            return message;
        }

        public virtual async Task Send<T>(T message)
        {
            ThrowIfDisposed();
            if(message is TMessage protocolMessage)
            {
                var data = _protocl.Encode(protocolMessage, Contex);
                await _networkStream.WriteAsync(data, _cancellationTokenSource.Token);
                LastSentMessageDateTime = DateTime.Now;
            }
        }


        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException($"Tcp connection {Id}");
            }
        }
        ~TcpSocketConnection() => Dispose(false);
        public void Dispose() => Dispose(true);

        private void Dispose(bool isDispose)
        {
            if(!_disposed)
            {
                _disposed = true;
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
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void OnMessage(Func<TMessage, Task> onMessageCallback)
        {
            _messageCallback = onMessageCallback;
        }
    }
}