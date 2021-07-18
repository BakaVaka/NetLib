using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IMessageDispatcher<TMessage, TContext>
            where TContext : IConnectionContext
    {
        public void Bind<TProtocol>(IConnection<TMessage, TProtocol, TContext> connection)
            where TProtocol : IProtocol<TMessage, TContext>;
    }
}