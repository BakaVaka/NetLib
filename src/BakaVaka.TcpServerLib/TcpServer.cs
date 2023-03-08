﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace BakaVaka.TcpServerLib;

/// <summary>
/// Базовая логика сервера
/// </summary>
public class TcpServer : IServer
{
    internal sealed class DefaultClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
        public DateTimeOffset NowUTC => DateTimeOffset.UtcNow;
    }

    private readonly ServerSettings _settings;
    private readonly ILogger<TcpServer> _logger;
    private readonly Func<Socket, IConnection> _connectionFactory;
    private readonly SemaphoreSlim _connectionLimiter;
    private readonly CancellationTokenSource _stopServerTokenSource = new();
    private readonly ConnectionManager _connectionManager;
    private readonly IClock _serverTimer = new DefaultClock();
    public TcpServer(ServerSettings settings,
        ILogger<TcpServer> logger,
        Func<Socket, IConnection> connectionFactory
        )
    {

        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger;
        _connectionFactory = connectionFactory;
        _connectionLimiter = new SemaphoreSlim(_settings.MaxConnections, _settings.MaxConnections);
        _connectionManager = new(
            _serverTimer,
            _settings.HeartbeatTimeout,
            _settings.DisconnectTimeout,
            _settings.IdleTimeout
        );

        _connectionManager.ConnectionComplete += (_, _) =>
        {
            _logger.LogTrace("Connection complete");
            _connectionLimiter.Release();
        };
    }

    public Task Run()
    {
        return Run(_stopServerTokenSource.Token);
    }

    public Task Stop()
    {
        foreach (var connection in _connectionManager.Connections)
        {
            connection.Abort();
        }
        _stopServerTokenSource.Cancel();
        return Task.CompletedTask;
    }

    private async Task Run(CancellationToken stopToken)
    {
        Socket acceptor = null;
        _logger.LogInformation("Server start running");
        try
        {

            while (!stopToken.IsCancellationRequested)
            {
                //если не смогли получить семафор в течении секунды
                //убьем серверный сокет и будем ждать, пока кто-нибудь не отвалится
                var canAccept = await _connectionLimiter.WaitAsync(1000, stopToken);
                if (!canAccept)
                {
                    _logger.LogWarning("Maximum connection reached. Stop accepting clients");
                    acceptor?.Close();
                    acceptor?.Dispose();
                    acceptor = null;

                    await _connectionLimiter.WaitAsync(stopToken);
                }

                if (!stopToken.IsCancellationRequested)
                {
                    //todo исправить
                    acceptor ??= CreateAcceptorSocket(_settings.ListeningEndPoint[0]);
                    var clientSocket = await acceptor.AcceptAsync();
                    _logger.LogTrace("New client accepted");
                    var connection = _connectionFactory(clientSocket);
                    if (_connectionManager.Bind(connection))
                    {
                        _logger.LogTrace("Client binded to server");
                        OnBinded(connection);
                    }
                }
            }

        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError($"Unexpected exception {ex}"); }
        _logger.LogInformation("Server complete running");
    }

    protected virtual void OnBinded(IConnection connection) { }
    private static Socket CreateAcceptorSocket(IPEndPoint listeningEndPoint)
    {
        Socket acceptor = new Socket(listeningEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        acceptor.Bind(listeningEndPoint);
        acceptor.Listen();
        return acceptor;
    }
}
