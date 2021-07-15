using System.IO;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    /// <summary>
    /// Абстрактный протокол
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщений(http, irc, etc)</typeparam>
    /// <typeparam name="TClientContext">Дополнительная инофрмация о клиенте, в случае если для кодирования-декодирования она нужна</typeparam>
    public interface IProtocol<TMessage, TClientContext>
    {
        public Task<TMessage> Decode(Stream inputStream, TClientContext context);
        public byte[] Encode(TMessage message, TClientContext clientContext);
    }
}