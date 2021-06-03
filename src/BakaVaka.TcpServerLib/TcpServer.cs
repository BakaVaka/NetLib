namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpServer
    {
        public event Action Stopped;
        public event Action Started;
        private readonly IPEndPoint _endPoint;
        private readonly object _startStopLocker = new();
        private CancellationTokenSource _serverStopToketSource;
        private SemaphoreSlim _connectionLimiter;
        private Func<ITransportConnection, CancellationToken, Task> _connetionHandler;
        private TcpSocketAcceptor _acceptor;

        public TcpServer(
            IPEndPoint endPoint,
            Func<ITransportConnection, CancellationToken, Task> connectionHandler,
            int maxConnection)
        {
            _endPoint = endPoint;
            _connectionLimiter = new SemaphoreSlim(maxConnection, maxConnection);
            _connetionHandler = connectionHandler;
            _acceptor = new TcpSocketAcceptor(
                socket =>
                {
                    return new TcpSocketConnection(socket, this);
                },
                _endPoint
                );
        }

        public void Start()
        {
            lock(_startStopLocker)
            {
                if(_acceptor.IsBinded)
                {
                    return;
                }
                _serverStopToketSource = new CancellationTokenSource();
                _acceptor.Bind();
            }
            RunAcceptLoop();
            Started?.Invoke();
        }
        public void Stop()
        {
            lock(_startStopLocker)
            {
                _acceptor.Unbind();
                _serverStopToketSource.Cancel();
                Stopped?.Invoke();
            }
        }

        private async void RunAcceptLoop()
        {
            try
            {
                while(!_serverStopToketSource.IsCancellationRequested)
                {
                    await _connectionLimiter.WaitAsync();
                    var clientConnection = await _acceptor.AcceptConnection(_serverStopToketSource.Token);
                    ProcessConnection(clientConnection);
                }
            }
            catch(SocketException ex) when (ex.SocketErrorCode != SocketError.Success) { }
            catch (ObjectDisposedException) { }
            catch (OperationCanceledException) { }
        }

        private async void ProcessConnection(ITransportConnection clientConnection)
        {
            try
            {
                await _connetionHandler(clientConnection, _serverStopToketSource.Token);
            }
            catch (Exception) { }
            finally 
            { 
                _connectionLimiter.Release(); 
                clientConnection.Dispose(); 
            }
        }
    }
}
