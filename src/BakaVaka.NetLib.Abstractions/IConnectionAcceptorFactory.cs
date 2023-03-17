using System.Net;

using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;

public interface IConnectionAcceptorFactory {
    public IListener Create(EndPoint endPoint);
}
