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
    public class ConnectionManager: IDisposable
    {
        public event EventHandler ConnectionComplete;

        /// <summary>
        /// таймер для проверки статуса соединений
        /// </summary>
        private readonly Timer _heartbeater;
        private readonly Func<IConnection> _connectionFactory;
        private readonly IServerTimer _serverTimer;
        private readonly TimeSpan _heartbeatInterval;
        private readonly TimeSpan _inactiveTimoutToClose;
        private readonly TimeSpan _inactiveTimoutToIdle;

        /// <summary>
        /// словарик для хранения активных подключений
        /// </summary>
        private ConcurrentDictionary<Guid, IConnection> _connectionsTable = new();


        public ConnectionManager(
            Func<IConnection> connectionFactory,
            IServerTimer serverTimer,
            TimeSpan heartbeatInterval, 
            TimeSpan inactiveTimoutToClose,
            TimeSpan inactiveTimoutToIdle
            )
        {
            _heartbeater = new(Heartbeat);
            _heartbeatInterval = heartbeatInterval;
            _connectionFactory = connectionFactory;
            _serverTimer = serverTimer;
            _inactiveTimoutToClose = inactiveTimoutToClose;
            _inactiveTimoutToIdle = inactiveTimoutToIdle;
            _heartbeater.Change(TimeSpan.Zero, heartbeatInterval);
        }

        public IEnumerable<IConnection> Connections => _connectionsTable.Values;

        public bool Bind(Socket clientSocket)
        {
            ThrowIfDisposed();
            var connection =  _connectionFactory();
            connection.Closed += (s,e) =>
            {
                if(_connectionsTable.TryRemove(connection.Id, out var _))
                {
                    Console.WriteLine("Connection unbind");
                    ConnectionComplete?.Invoke(this, EventArgs.Empty);
                }
            };
            if (_connectionsTable.TryAdd(connection.Id, connection))
            {
                connection.BindSocket(clientSocket);
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



                    if (now - recent >= _inactiveTimoutToClose)
                    {
                        inactiveConnections.Add(connection);
                        continue;
                    }                    

                    if ( now - recent >= _inactiveTimoutToIdle)
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