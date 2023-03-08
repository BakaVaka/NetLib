using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IAcceptor
    {
        public EndPoint BindedEndPoint { get; }
        public void Bind();
        public void Unbind();
        public ValueTask<IConnection> Accept(CancellationToken cancellationToken);
    }
}
