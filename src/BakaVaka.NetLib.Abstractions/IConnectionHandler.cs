namespace BakaVaka.NetLib.Abstractions;
public interface IConnectionHandler {
    public Task Handle(IConnection connection, CancellationToken cancellationToken = default);
}