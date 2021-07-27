using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    /// <summary>
    /// Отвечает за то чтобы следить за неактивными сессиями
    /// Предоставляет доступ к активным подключениям
    /// </summary>
    internal class ConnectionManager: IDisposable
    {
        public event EventHandler ConnectionComplete;

        /// <summary>
        /// таймер для проверки статуса соединений
        /// </summary>
        private readonly Timer _heartbeater;
        private readonly IServerTimer _serverTimer;
        private readonly TimeSpan _heartbeatInterval;
        private readonly TimeSpan _inactiveTimoutToClose;
        private readonly TimeSpan _inactiveTimoutToIdle;

        /// <summary>
        /// словарик для хранения активных подключений
        /// </summary>
        private ConcurrentDictionary<Guid, IConnection> _connectionsTable = new();


        public ConnectionManager(
            IServerTimer serverTimer,
            TimeSpan heartbeatInterval, 
            TimeSpan inactiveTimoutToClose,
            TimeSpan inactiveTimoutToIdle
            )
        {
            _heartbeater = new(Heartbeat);
            _heartbeatInterval = heartbeatInterval;
            _serverTimer = serverTimer;
            _inactiveTimoutToClose = inactiveTimoutToClose;
            _inactiveTimoutToIdle = inactiveTimoutToIdle;
            _heartbeater.Change(TimeSpan.Zero, heartbeatInterval);
        }

        public IEnumerable<IConnection> Connections => _connectionsTable.Values;

        public bool Bind(IConnection connection)
        {
            ThrowIfDisposed();
            connection.Closed += (s,e) =>
            {
                if(_connectionsTable.TryRemove(connection.Id, out var _))
                {
                    ConnectionComplete?.Invoke(this, EventArgs.Empty);
                }
            };
            if (_connectionsTable.TryAdd(connection.Id, connection))
            {
                connection.Open();
                return true;
            }
            return false;
        }

 

        private void Heartbeat(object _)
        {
            _heartbeater.Change(-1, -1);
            try
            {
                List<IConnection> inactiveConnections = new();
                List<IConnection> idleConnctions = new();

                var now = _serverTimer.ServerTime;
                foreach(var(key, connection) in _connectionsTable)
                {
                    
                    var recent = connection.LastAcceptedMessageDateTime > connection.LastSentMessageDateTime
                        ? connection.LastSentMessageDateTime
                        : connection.LastAcceptedMessageDateTime;

                    //соединение открыто давно и при этом не получило ни одного сообщения за время неактивности
                    if (now - recent >= _inactiveTimoutToClose && now - connection.Opened >= _inactiveTimoutToClose)
                    {
                        inactiveConnections.Add(connection);
                        continue;
                    } 
                    else if ( now - recent >= _inactiveTimoutToIdle)
                    {
                        idleConnctions.Add(connection);
                        continue;
                    }

                }

                foreach(var connection in idleConnctions)
                {
                    connection.OnIdle();
                }
                foreach(var connection in inactiveConnections)
                {
                    connection.Close();
                }
            }
            catch { }
            finally
            {
                _heartbeater.Change(_heartbeatInterval, _heartbeatInterval);
            }
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Connection manager", "Connection manager is disposed");
            }
        }
        // dispose

        private bool _disposed;
        ~ConnectionManager() => Dispose(false);
        public void Dispose() => Dispose(true);
        private void Dispose(bool isDispose)
        {
            if(_disposed)
            {
                _disposed = true;
                _heartbeater.Dispose();
                if(isDispose)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}