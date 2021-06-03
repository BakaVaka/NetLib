namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConnectionAcceptor : IDisposable
    {
        public bool IsBinded { get; }
        public ValueTask Bind();
        public ValueTask Unbind();
        public ValueTask<ITransportConnection> AcceptConnection(CancellationToken cancellationToken);
    }
}
