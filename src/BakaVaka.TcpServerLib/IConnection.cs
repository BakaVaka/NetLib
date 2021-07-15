using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IConnection
        <
            TMessage, 
            TProtocol, 
            TClientContex
        > 
        : IDisposable
        where TProtocol : IProtocol<TMessage, TClientContex>
        where TClientContex : IConnectionContext, new()

    {
        public event EventHandler Closed;
        public void Close();
        public string ID { get; }
        public TClientContex Contex { get; }
        public EndPoint LocalEndPoint { get; }
        public EndPoint RemoteEndPoint { get; }
        public DateTime LastRecievedDateTime { get; }
        public DateTime LastSentDateTime { get; }
        public Task<TMessage> Receive(CancellationToken cancellationToken = default);
        public Task Send<T>(T message);
    }
}