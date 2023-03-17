using System.Collections.Concurrent;

using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;

/// <summary>
/// Отвечает за то чтобы следить за неактивными сессиями
/// Предоставляет доступ к активным подключениям
/// </summary>
internal class ConnectionManager : IDisposable {
    public event EventHandler ConnectionComplete;

    private readonly IClock _serverTimer;
    private readonly TimeSpan _heartbeatInterval;
    private readonly TimeSpan _inactiveTimoutToClose;
    private readonly TimeSpan _inactiveTimoutToIdle;

    /// <summary>
    /// словарик для хранения активных подключений
    /// </summary>
    private ConcurrentDictionary<Guid, IConnection> _connectionsTable = new();


    public ConnectionManager(
        IClock serverTimer,
        TimeSpan heartbeatInterval,
        TimeSpan inactiveTimoutToClose,
        TimeSpan inactiveTimoutToIdle
        ) {
        _serverTimer = serverTimer;
        _inactiveTimoutToClose = inactiveTimoutToClose;
        _inactiveTimoutToIdle = inactiveTimoutToIdle;
    }

    public IEnumerable<IConnection> Connections => _connectionsTable.Values;

    public bool Bind(IConnection connection) {
        ThrowIfDisposed();
        if( _connectionsTable.TryAdd(connection.Id, connection) ) {
            connection.Start();
            return true;
        }
        return false;
    }



    private void Heartbeat(object _) {
        _heartbeater.Change(-1, -1);
        try {
            //List<IConnection> inactiveConnections = new();
            //List<IConnection> idleConnctions = new();

            //var now = _serverTimer.Now;
            //foreach (var (key, connection) in _connectionsTable)
            //{

            //    var recent = connection.LastAcceptedMessageDateTime > connection.LastSentMessageDateTime
            //        ? connection.LastSentMessageDateTime
            //        : connection.LastAcceptedMessageDateTime;

            //    //соединение открыто давно и при этом не получило ни одного сообщения за время неактивности
            //    if (now - recent >= _inactiveTimoutToClose && now - connection.OpenedAt >= _inactiveTimoutToClose)
            //    {
            //        inactiveConnections.Add(connection);
            //        continue;
            //    }
            //    else if (now - recent >= _inactiveTimoutToIdle)
            //    {
            //        idleConnctions.Add(connection);
            //        continue;
            //    }

            //}

            //foreach (var connection in idleConnctions)
            //{
            //    connection.OnIdle();
            //}
            //foreach (var connection in inactiveConnections)
            //{
            //    connection.Abort();
            //}
        }
        catch { }
        finally {
            _heartbeater.Change(_heartbeatInterval, _heartbeatInterval);
        }
    }


    private void ThrowIfDisposed() {
        if( _disposed ) {
            throw new ObjectDisposedException("Connection manager", "Connection manager is disposed");
        }
    }
    // dispose

    private bool _disposed;
    ~ConnectionManager() => Dispose(false);
    public void Dispose() => Dispose(true);
    private void Dispose(bool isDispose) {
        if( _disposed ) {
            _disposed = true;
            _heartbeater.Dispose();
            if( isDispose ) {
                GC.SuppressFinalize(this);
            }
        }
    }
}