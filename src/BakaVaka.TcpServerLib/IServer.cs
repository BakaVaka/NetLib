namespace BakaVaka.TcpServerLib
{
    using System.Threading.Tasks;
    public interface IServer
    {
        public Task Start();
        public Task Stop();
    }
}
