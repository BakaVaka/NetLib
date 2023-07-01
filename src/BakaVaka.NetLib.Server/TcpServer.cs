using System.Net;

using BakaVaka.NetLib.Abstractions;
using BakaVaka.TcpServerLib;
using BakaVaka.TcpServerLib.Features;

namespace BakaVaka.NetLib.Server;

/// <summary>
/// Базовая логика сервера
/// </summary>
public class TcpServer : IServer {

    const int SERVER_WAITING_FOR_START = 0;
    const int SERVER_STARTING = 1;
    const int SERVER_RUN = 2;
    const int SERVER_STOPING = 3;


    private readonly TcpServerSettings _settings;
    private readonly ConnectionHandler _handler;
    private readonly Heartbeat _heartbeat;
    private List<IListener> _listeners = new();
    private int _serverState;
    public TcpServer(TcpServerSettings settings, ConnectionHandler handler) {
        _settings = settings;
        _handler = handler;
        _heartbeat = new Heartbeat(settings.Clock, new Heartbeat.HeartbeatSettings() { HeartbeatInterval = TimeSpan.FromSeconds(1) });
    }
    public async Task StartAsync(CancellationToken cancellationToken = default) {
        
        if(Interlocked.CompareExchange(ref _serverState, SERVER_STARTING, SERVER_WAITING_FOR_START) != SERVER_WAITING_FOR_START ) {
            throw new Exception("Invalid state");
        }
        ValidateSettings(_settings);

        _listeners = _settings.Listen
            .Select(x => (IListener)new SocketAccpetor(new IPEndPoint(IPAddress.IPv6Any, x)))
            .ToList();

        foreach(var listener in _listeners) {
            listener.Bind();
            listener.ConnectionAccepted += OnConnectionAccepted;
            listener.Run();
        }
        _heartbeat.Start();
    }

    
    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if(Interlocked.CompareExchange(ref _serverState, SERVER_STOPING, SERVER_RUN) != SERVER_RUN ) {
            throw new Exception("Invalid state");
        }
        _heartbeat.Stop();
        foreach(var listeners in _listeners) {
            listeners.Unbind();
        }

        _serverState = SERVER_WAITING_FOR_START;
    }
    private void ValidateSettings(TcpServerSettings settings) {
        if(settings.Listen.Length == 0 ) {
            throw new ArgumentException("At least 1 port required for start server");
        }

        if(settings.IdleTimout > settings.DisconnectionTimout ) {
            throw new ArgumentException("Invalid timeout settings");
        }

        //todo other checks
    }

    private async void OnConnectionAccepted(IConnection connection) {


        if( connection.Features.HasFeatuer<IHeartbeatFeature>() ) {
            var heartBeatFeature = connection.Features.Get<IHeartbeatFeature>();
            _heartbeat.Tick += (time) => heartBeatFeature.OnHeartbeat(time);
        }

        connection.Start();

        try {
            await _handler(connection);
        }
        catch( Exception ) { }


        if( connection.Features.HasFeatuer<IHeartbeatFeature>() ) {
            var heartBeatFeature = connection.Features.Get<IHeartbeatFeature>();
            _heartbeat.Tick -= (time) => heartBeatFeature.OnHeartbeat(time);
        }

    }

}
