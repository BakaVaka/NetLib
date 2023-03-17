using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetlLib.Hosting;

public interface IBakaApplication<TContext> {

    public TContext CreateContext(IConnection connection);
    public Task RunApplication(TContext context, CancellationToken cancellationToken);

}