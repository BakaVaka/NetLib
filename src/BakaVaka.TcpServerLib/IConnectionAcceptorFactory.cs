namespace BakaVaka.TcpServerLib
{
    public interface IConnectionAcceptorFactory
    {
        public IConnectionAcceptor Create(ServerSettings settings);

    }
}
