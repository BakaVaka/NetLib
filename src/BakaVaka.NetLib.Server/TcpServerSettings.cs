namespace BakaVaka.TcpServerLib;

public class TcpServerSettings {
    public TcpServerSettings(
        int[] listen,
        long connectionLimit,
        TimeSpan idleTimout,
        TimeSpan disconnectionTimout) {
        Listen = listen ?? throw new ArgumentNullException(nameof(listen));
        ConnectionLimit = connectionLimit;
        IdleTimout = idleTimout;
        DisconnectionTimout = disconnectionTimout;
    }

    public int[] Listen { get; }
    public long ConnectionLimit { get; }
    public TimeSpan IdleTimout { get; }
    public TimeSpan DisconnectionTimout { get; }
}
