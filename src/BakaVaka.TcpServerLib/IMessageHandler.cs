using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IMessageHandler<TMessage, TProtocol> where TProtocol : IProtocol<TMessage>, new()
    {
        public Task HandleMessage(TMessage message, IConnection<TMessage,TProtocol> connection);
    }
}