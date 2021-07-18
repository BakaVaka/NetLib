using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IConnection
    {
        public Guid Id { get; }
        event EventHandler Closed;
        public DateTime LastAcceptedMessageDateTime { get; }
        public DateTime LastSentMessageDateTime { get; }
        public void BindSocket(Socket socket);
        public Task Send<T>(T message);
        void OnIdle();
        void Close();
        void Dispose();
    }
}