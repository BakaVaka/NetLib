using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib;

public class TcpSocketConnection : IConnection
{

    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _closed;
    private EndPoint _localEndPoint;
    private EndPoint _remoteEndPoint;
    private Socket _clientSocket;
    private Exception? _closeReason;
    private Task? _connectionRunTask;
    private readonly ITransportPair _transportPair;
    private readonly SocketReceiver _receiver;
    private readonly SocketSender _sender;
    private bool _disposed;

    private Task _runTask;
    public TcpSocketConnection(Socket cleintSocket)
    {

        _localEndPoint = cleintSocket.LocalEndPoint;
        _remoteEndPoint = cleintSocket.RemoteEndPoint;
        _clientSocket = cleintSocket;
        _transportPair = new DefaultTransportPair(new Pipe(), new Pipe());
        _receiver = new SocketReceiver(_clientSocket, _transportPair.In.Out);
        _sender = new SocketSender(_clientSocket, _transportPair.Out.In);
        Transport = _transportPair.In;

    }
    public EndPoint LocalEndPoint => _localEndPoint;
    public EndPoint RemoteEndPoint => _remoteEndPoint;
    public ITransport Transport { get; internal set; }
    public Guid Id { get; }
    public DateTimeOffset? OpenedAt { get; }

    public void Start()
    {
        var receiveTask = _receiver.Receive(_cancellationTokenSource.Token);
        var sendTask = _sender.Send(_cancellationTokenSource.Token);
        _runTask = Task.WhenAll(receiveTask, sendTask);
    }

    public void Abort(Exception reason = null)
    {
        if (!_closed)
        {
            _closed = true;
            _cancellationTokenSource.Cancel();
            _closeReason = reason;
        }
    }


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
            _cancellationTokenSource.Dispose();
            _clientSocket.Dispose();
            if (isDispose)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}