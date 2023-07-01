namespace BakaVaka.NetlLib.Hosting;
/// <summary>
/// Хост сервера
/// </summary>
public class Host {
    private IServiceProvider _serviceProvider;
    private int _started = new();

    public Task StartAsync(CancellationToken cancellationToken = default) {
        return Task.CompletedTask;
    }

    private Task StopAsync(CancellationToken cancellationToken = default) {
        var started = Interlocked.CompareExchange(ref _started, 0, 1);
        if(started == 0 ) {
            throw new InvalidOperationException("Not started yet");
        }

        return Task.CompletedTask;
    }
}
