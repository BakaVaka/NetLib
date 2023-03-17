namespace BakaVaka.TcpServerLib;

internal sealed class ServerContext {
    public ServerContext(IClock clock, TcpServerSettings serverSettings, IServiceProvider serviceProvider) {
        Clock = clock;
        ServerSettings = serverSettings;
        ServiceProvider = serviceProvider;
    }

    public IClock Clock { get; }
    public TcpServerSettings ServerSettings { get; }
    public IServiceProvider ServiceProvider { get; }

}