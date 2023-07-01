using System.Net;

namespace BakaVaka.NetLib.Abstractions;

public interface IConnectionAcceptorFactory {
    public IListener Create(EndPoint endPoint);
}
