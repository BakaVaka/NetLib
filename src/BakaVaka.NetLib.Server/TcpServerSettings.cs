using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Server;

public class TcpServerSettings {
    public TcpServerSettings(
        int[] listen,
        long connectionLimit,
        IClock clock,
        TimeSpan idleTimout,
        TimeSpan disconnectionTimout) {
        Listen = listen ?? throw new ArgumentNullException(nameof(listen));
        ConnectionLimit = connectionLimit;
        Clock = clock;
        IdleTimout = idleTimout;
        DisconnectionTimout = disconnectionTimout;
    }

    public int[] Listen { get; }
    public long ConnectionLimit { get; }
    public IClock Clock { get; }
    public TimeSpan IdleTimout { get; }
    public TimeSpan DisconnectionTimout { get; }
}
