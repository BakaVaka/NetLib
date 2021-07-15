namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpServer<TMessage, TProtocol, TContext>
        where TProtocol : IProtocol<TMessage, TContext>
        where TContext : IConnectionContext, new()
    {
        private IPEndPoint _endPoint;
        private readonly Func<Socket, IConnection<TMessage, TProtocol, TContext>> _connectionFactory;
        private readonly Func<TContext, TMessage, Task> _messageHandler;
        private SemaphoreSlim _connectionLimiter;
        private CancellationTokenSource _serverStopToketSource = new();
        private Socket _acceptor;

        public TcpServer(
            IPEndPoint endPoint,
            Func<Socket, IConnection<TMessage, TProtocol, TContext>> connectionFactory,
            Func<TContext, TMessage, Task> messageHandler,
            int maxConnection)
        {
            _endPoint = endPoint;
            _connectionFactory = connectionFactory;
            _messageHandler = messageHandler;
            _connectionLimiter = new SemaphoreSlim(maxConnection, maxConnection);
        }

        public Task Run()
        {
            CreateAcceptor();
            return Task.Run(RunAcceptLoop, _serverStopToketSource.Token);
        }

        private void CreateAcceptor()
        {
            _acceptor = new(SocketType.Stream, ProtocolType.Tcp);
            _acceptor.Bind(_endPoint);
            _acceptor.Listen();
        }

        private async Task RunAcceptLoop()
        {
            try
            {
                while(!_serverStopToketSource.IsCancellationRequested)
                {
                    await _connectionLimiter.WaitAsync();
                    var clientSocket = await Task.Run(_acceptor.AcceptAsync,_serverStopToketSource.Token);
                    var connection = _connectionFactory(clientSocket);
                    connection.Closed += (_, _) =>
                    {
                        Console.WriteLine("Connection closed");
                        _connectionLimiter.Release();
                    };
                    ProcessConnection(connection);
                }
            }
            catch(SocketException ex) when (ex.SocketErrorCode != SocketError.Success) { }
            catch (ObjectDisposedException) { }
            catch (OperationCanceledException) { }
        }

        private async void ProcessConnection(IConnection<TMessage, TProtocol, TContext> connection)
        {
            try
            {
                while(true)
                {
                    var message = await connection.Receive();
                    await _messageHandler(connection.Contex, message);
                }
            }
            catch (Exception) { connection.Close(); }
        }
    }
}
