using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

using BakaVaka.NetLib.Abstractions;
using BakaVaka.NetLib.Shared;

namespace BakaVaka.TcpServerLib;

public class SocketConnection : IConnection {

    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _closed;
    private Socket _clientSocket;
    private Exception? _closeReason;
    private Task? _connectionRunTask;
    private readonly ITransportPair _transportPair;
    private readonly SocketConnectionReceiver _receiver;
    private readonly SocketConnectionSender _sender;
    private bool _disposed;

    private Task _runTask;
    public SocketConnection(Socket clientSocket) {
        Items = new ItemStore();
        Features = new FeatureCollection();

        LocalEndPoint = clientSocket.LocalEndPoint;
        RemoteEndPoint = clientSocket.RemoteEndPoint;
        _clientSocket = clientSocket;
        _transportPair = new DefaultTransportPair(new Pipe(), new Pipe());
        _receiver = new SocketConnectionReceiver(_clientSocket, _transportPair.In.Out);
        _sender = new SocketConnectionSender(_clientSocket, _transportPair.Out.In);
        Transport = _transportPair.In;

    }
    public EndPoint LocalEndPoint { get; }
    public EndPoint RemoteEndPoint { get; }
    public ITransport Transport { get; internal set; }
    public Guid Id { get; }
    public DateTimeOffset? StartedAt { get; }
    public IItemStore Items { get; }
    public IFeatureCollection Features { get; }

    public void Start() {
        var receiveTask = _receiver.Receive(_cancellationTokenSource.Token);
        var sendTask = _sender.Send(_cancellationTokenSource.Token);
        _runTask = Task.WhenAll(receiveTask, sendTask);
    }

    public void Abort(Exception reason = null) {
        if( !_closed ) {
            _closed = true;
            _cancellationTokenSource.Cancel();
            _closeReason = reason;
        }
    }


    private void ThrowIfDisposed() {
        if( _disposed ) {
            throw new ObjectDisposedException($"Tcp connection {Id}");
        }
    }
    ~SocketConnection() => Dispose(false);
    public void Dispose() => Dispose(true);

    private void Dispose(bool isDispose) {
        if( !_disposed ) {
            _disposed = true;
            _cancellationTokenSource.Dispose();
            _clientSocket.Dispose();
            if( isDispose ) {
                GC.SuppressFinalize(this);
            }
        }
    }
}