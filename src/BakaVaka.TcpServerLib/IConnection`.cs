using System;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib
{
    public interface IConnection<TMessage, TProtocol, TClientContex> : IConnection
        where TProtocol : IProtocol<TMessage, TClientContex>
        where TClientContex : IConnectionContext
    {
        TClientContex Contex { get; }
        void OnMessage(Func<TMessage, Task> messageCallbach);
    }
}