namespace BakaVaka.TcpServerLib
{
    using System.Threading.Tasks;
    public interface IServer
    {
        public Task Run();
        public Task Stop();
    }
}
