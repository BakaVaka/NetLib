using System.Collections.Concurrent;

using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Shared;
internal class ConnectionsStore {
    private ConcurrentDictionary<Guid, WeakReference<IConnection>> _connections = new();
    public bool TryGetById(Guid id, out IConnection? connection) {
        connection = default;
        if( _connections.TryGetValue(id, out var @ref) ) {
            if( @ref.TryGetTarget(out connection) ) {
                return true;
            }
            //celan, cause connection is lost now(((
            _connections.TryRemove(id, out _);
        }
        return false;
    }
    public bool TryRegister(Guid id, IConnection connection) {
        ArgumentNullException.ThrowIfNull(connection);
        var @ref = new WeakReference<IConnection>(connection);
        return _connections.TryAdd(id, @ref);
    }
    public void RemoveById(Guid id) {
        _connections.TryRemove(id, out _);
    }
    public IEnumerable<IConnection> GetConnections() {
        if( _connections.Count == 0 ) {
            yield break;
        }

        //UwU
        var connectionInfo = Array.Empty<KeyValuePair<Guid, WeakReference<IConnection>>>();
        lock( _connections ) {
            connectionInfo = _connections.ToArray();
        }

        foreach( var (key, val) in _connections ) {
            if( val.TryGetTarget(out var connection) ) {
                yield return connection;
            }
            else {
                RemoveById(key);
            }
        }
    }
}
