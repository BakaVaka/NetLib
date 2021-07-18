namespace BakaVaka.TcpServerLib
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Базовая логика сервера
    /// </summary>
    public class TcpServerBase : IServer
    {
        public class ServerTimer : IServerTimer
        {
            public DateTime ServerTime => DateTime.Now;
        }

        private readonly ServerSettings _settings;
        private readonly ILogger<TcpServerBase> _logger;
        private readonly Func<IConnection> _connectionFactory;

        private readonly Func<Socket> _acceptorFactory;
        private readonly SemaphoreSlim _connectionLimiter;
        private readonly CancellationTokenSource _stopServerTokenSource = new();
        private readonly ConnectionManager _connectionManager;
        private readonly IServerTimer _serverTimer = new ServerTimer();
        public TcpServerBase(ServerSettings settings, 
            ILogger<TcpServerBase> logger,
            Func<IConnection> connectionFactory
            )
        {

            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
            _connectionFactory = connectionFactory;
            _connectionLimiter = new SemaphoreSlim(_settings.MaxConnections, _settings.MaxConnections);
            _acceptorFactory = () =>
            {
                Socket acceptor = new Socket(_settings.ListeningEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                acceptor.Bind(_settings.ListeningEndPoint);
                acceptor.Listen();
                return acceptor;
            };
            _connectionManager = new(
                _connectionFactory,
                _serverTimer,
                _settings.HeartbeatTimout,
                _settings.DisconnectTimout,
                _settings.IdleTimout
            );

            _connectionManager.ConnectionComplete += (_, _) =>
            {
                _logger.LogTrace("Connection complete");
                _connectionLimiter.Release();
            };
        }


        public Task Start()
        {
            return Run(_stopServerTokenSource.Token);
        }

        public Task Stop()
        {
            _stopServerTokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken stopToken)
        {
            Socket acceptor = null;
            _logger.LogInformation("Server start accepting clients");
            try
            {

                while (!stopToken.IsCancellationRequested)
                {
                    //если не смогли получить семафор в течении секунды
                    //убьем серверный сокет и будем ждать, пока кто-нибудь не отвалится
                    var canAccept = await _connectionLimiter.WaitAsync(1000, stopToken);
                    if(!canAccept)
                    {
                        _logger.LogWarning("Maximum connection reached. Stop accepting clients");
                        acceptor?.Close();
                        acceptor?.Dispose();
                        acceptor = null;

                        await _connectionLimiter.WaitAsync(stopToken);
                    }

                    if(!stopToken.IsCancellationRequested)
                    {
                        acceptor ??= _acceptorFactory();
                        var clientSocket = await acceptor.AcceptAsync();
                        _logger.LogTrace("New client accepted");
                        if(_connectionManager.Bind(clientSocket))
                        {
                            _logger.LogTrace("Client binded to server");
                        }
                    }
                }

            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { _logger.LogError($"Unexpected exception {ex}"); }
            _logger.LogInformation("Server complete running");
        }

        protected virtual void OnBinded(IConnection connection){}
    }
}
