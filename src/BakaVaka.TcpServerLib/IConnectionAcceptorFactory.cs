using System.Net;

namespace BakaVaka.TcpServerLib
{
    public interface IConnectionAcceptorFactory
    {
        public IAcceptor Create(EndPoint endPoint);
    }
}
