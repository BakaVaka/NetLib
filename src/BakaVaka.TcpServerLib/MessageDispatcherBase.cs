namespace BakaVaka.TcpServerLib
{
    using System.Threading.Tasks;

    public abstract class MessageDispatcherBase<TMessage, TContext> : IMessageDispatcher<TMessage, TContext>
        where TContext : IConnectionContext
    {
        public void Bind<TProtocol>(IConnection<TMessage, TProtocol, TContext> connection)
            where TProtocol : IProtocol<TMessage, TContext>
        {
            connection.OnMessage(async (message) => await Dispatch(message, connection, connection.Contex));
        }

        protected abstract Task Dispatch<TProtocol>(TMessage message,
            IConnection<TMessage, TProtocol, TContext> connection,
            TContext contex) where TProtocol : IProtocol<TMessage, TContext>;
    }
}
