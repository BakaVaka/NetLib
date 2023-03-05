using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IConnectionAcceptor
    {
        public Task<IConnection> Accept();
    }
}
