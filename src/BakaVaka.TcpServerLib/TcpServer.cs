namespace BakaVaka.TcpServerLib
{
    using Microsoft.Extensions.Logging;
    using System;

    public abstract class TcpServer<TMessage, TProtocol, TContext>
        : TcpServerBase
        where TProtocol : IProtocol<TMessage, TContext>
        where TContext : IConnectionContext, new()
    {
        public TcpServer(
                ServerSettings settings, 
                ILogger<TcpServerBase> logger,
                IMessageDispatcher<TMessage, TContext> messageDispatcher,
                Func<IConnection<TMessage, TProtocol, TContext>> connectionFactory
            ) : base(settings, logger, 
                () => 
                { 
                    var connection = connectionFactory();
                    messageDispatcher.Bind(connection);
                    return connection;
                })
        {}
    }
}
