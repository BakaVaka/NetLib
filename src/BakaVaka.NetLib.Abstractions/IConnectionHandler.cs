using BakaVaka.NetLib.Abstractions;
using BakaVaka.TcpServerLib.Events;

namespace BakaVaka.TcpServerLib;
public interface IConnectionHandler {
    public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
    public Task Handle(IConnection connection, CancellationToken cancellationToken = default);
}