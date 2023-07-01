using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Server;

internal sealed class TcpServerContext {
    public TcpServerContext(IClock clock, TcpServerSettings serverSettings, IServiceProvider serviceProvider) {
        Clock = clock;
        ServerSettings = serverSettings;
        ServiceProvider = serviceProvider;
    }

    public IClock Clock { get; }
    public TcpServerSettings ServerSettings { get; }
    public IServiceProvider ServiceProvider { get; }

}