using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IConnectionHandler
    {
        public Task Handle(IConnection connection);
    }
}