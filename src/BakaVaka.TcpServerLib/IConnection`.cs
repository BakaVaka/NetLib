using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib{

    public interface IConnection<TMessage, TProtocol> : IConnection
        where TProtocol : IProtocol<TMessage>
    {
        public Task Send(TMessage message);
    }
}