using System;

namespace BakaVaka.TcpServerLib
{
    public interface IConnection : IDisposable
    {
        public Guid Id { get; }
        public event EventHandler Closed;
        public DateTime LastAcceptedMessageDateTime { get; }
        public DateTime LastSentMessageDateTime { get; }
        public DateTime Opened { get; }
        public void OnIdle();
        public void Open();
        public void Close();
    }
}