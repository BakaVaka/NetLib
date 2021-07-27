namespace BakaVaka.TcpServerLib
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Sockets;

    public abstract class TcpServer<TMessage, TProtocol>
        : TcpServerBase
        where TProtocol : IProtocol<TMessage>
    {
        public TcpServer(
                ServerSettings settings, 
                ILogger<TcpServerBase> logger,
                Func<Socket, IConnection<TMessage, TProtocol>> connectionFactory
            ) : base(settings, logger, connectionFactory)
        {}
    }
}
