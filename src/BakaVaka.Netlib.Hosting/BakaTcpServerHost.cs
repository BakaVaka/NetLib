using BakaVaka.TcpServerLib;

namespace BakaVaka.NetlLib.Hosting;
public class BakaTcpServerHost {
    private IServiceProvider _serviceProvider;
    private readonly Heartbeat _heartbeat;
    private int _started = new();

    public Task Run<TContext>(IBakaApplication<TContext> application, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

}
