using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    /// <summary>
    /// Абстрактный протокол, например HTTP, IRC, SNMP, etc
    /// </summary>
    public interface IProtocol<TMessage>
    {
        public Task<TMessage> Decode(Stream inputStream, IConnection connection, CancellationToken cancellationToken = default);
        public byte[] Encode(TMessage message, IConnection connection);
    }
}