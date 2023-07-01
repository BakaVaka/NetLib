using System.Net;
using System.Net.Sockets;

using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;

/// <summary>
/// Простой слушатель TCP подключений
/// Принимает соедниения, оборачивает в IConnection
/// </summary>
public class SocketAccpetor : IListener {
    private Socket? _socket;
    private CancellationTokenSource _cancellationTokenSource;

    public event Action<IConnection> ConnectionAccepted = (_) => { };

    public SocketAccpetor(IPEndPoint endPoint) {
        BindedEndPoint = endPoint;
    }
    public EndPoint BindedEndPoint { get; }
    public bool IsBinded => _socket != null;
    public async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default) {
        var clientSocket = await _socket.AcceptAsync(cancellationToken);
        var connection = new SocketConnection(clientSocket);
        return connection;
    }

    public void Bind() {
        if( _socket != null ) {
            throw new InvalidOperationException("Socket is binded already");
        }
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(BindedEndPoint);
        _socket.Listen();
        _cancellationTokenSource = new CancellationTokenSource();
    }
    public void Unbind() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _socket?.Dispose();
        _socket = null;

    }

    public async Task Run(CancellationToken cancellationToken = default) {
        var stopListenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        var stopToken = stopListenSource.Token;
        while( !stopToken.IsCancellationRequested ) {
            try {
                var client = await _socket.AcceptAsync(stopToken);
                if( client is not null ) {
                    var connection = new SocketConnection(client);
                    ConnectionAccepted?.Invoke(connection);
                }
            }
            catch( OperationCanceledException ) { break; }
            catch( ObjectDisposedException ) { break; }
        }

    }
}
