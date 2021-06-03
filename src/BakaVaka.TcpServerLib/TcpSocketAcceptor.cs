namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpSocketAcceptor : IConnectionAcceptor
    {
        private Socket _socket;
        private readonly int _backlog;
        private readonly Func<Socket, ITransportConnection> _transportFactory;
        private readonly EndPoint _endPoint;
        private readonly bool _useDualMode;

        public TcpSocketAcceptor(
            Func<Socket, ITransportConnection> transportFactory,
            EndPoint endPoint, 
            bool useDualMode = true, 
            int backlog = 1000)
        {
            if (transportFactory is null)
            {
                throw new ArgumentNullException(nameof(transportFactory));
            }

            if (endPoint is null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (endPoint is not IPEndPoint)
            {
                throw new ArgumentException($"Only {typeof(IPEndPoint).FullName} supported now");
            }

            _transportFactory = transportFactory;
            _endPoint = endPoint;
            _backlog = backlog;
            _useDualMode = useDualMode;

        }

        public bool IsBinded => _socket is not null;

        public async ValueTask<ITransportConnection> AcceptConnection(CancellationToken cancellationToken)
        {
            if(_socket is null)
            {
                throw new InvalidOperationException("Before accept connection you must bind socket");
            }
            Socket acceptor = _socket;
            var client = await acceptor.AcceptAsync();
            return _transportFactory(client);
        }

        public ValueTask Bind()
        {
            if (IsBinded)
            {
                throw new InvalidOperationException("Already binded");
            }
            _socket = CreateSocket();
            return ValueTask.CompletedTask;
        }

        public ValueTask Unbind()
        {
            //ничего страшного нет в том, чтобы просто выйти из метода тихо, если и так отбиндились
            if (!IsBinded)
            {
                return ValueTask.CompletedTask;
            }
            _socket.Dispose();
            _socket = null;
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        private Socket CreateSocket()
        {
            //todo имплементировать UnixDomainSocketEndPoint
            switch (_endPoint)
            {
                case IPEndPoint ip:
                    {
                        Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        if(ip.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            socket.DualMode = _useDualMode;
                        }
                        socket.Bind(ip);
                        socket.Listen(_backlog);
                        return socket;
                    }
                default:
                    throw new NotSupportedException($"Only {typeof(IPEndPoint).FullName} supported now");
            }
        }
    }
}
