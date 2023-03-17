namespace BakaVaka.NetLib.Abstractions;
public interface IServer {
    public Task StartAsync( CancellationToken cancellationToken = default );
    public Task StopAsync( CancellationToken cancellationToken = default );
    public Task Run(CancellationToken cancellationToken = default );
}
