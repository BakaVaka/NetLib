using System;
using System.Net;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface ITransportConnection : IDisposable
    {
        public EndPoint LocalEndPoint { get; }
        public EndPoint RemoteEndPoint { get; }
        public ValueTask<int> Receve(Memory<byte> buffer);
        public ValueTask<int> Send(ReadOnlyMemory<byte> message);
    }
}