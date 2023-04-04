using System.Net;
using System.Net.Sockets;

using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;

/// <summary>
/// Простой слушатель TCP подключений
/// Принимает соедниения, оборачивает в IConnection
/// </summary>
public class TcpSocketAccpetor : IListener {
    private Socket? _socket;
    public TcpSocketAccpetor(IPEndPoint endPoint) {
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
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(BindedEndPoint);
        _socket.Listen();
    }

    public async Task Run(ConnectionHandler connectionHandler, CancellationToken cancellationToken) {
        while( !cancellationToken.IsCancellationRequested ) {
            try {
                var client = await _socket.AcceptAsync(cancellationToken);
                if(client is not null ) {
                    var 
                }
            }
            catch( Exception ) {

            }
        }
    
    }

    public void Unbind() {
        _socket.Dispose();
        _socket = null;
    }
}
