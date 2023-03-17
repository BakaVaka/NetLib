namespace BakaVaka.TcpServerLib;

public class TcpServerSettings {
    public TcpServerSettings(
        int[] listen,
        long connectionLimit,
        TimeSpan idleTimout,
        TimeSpan disconnectionTimout,
        bool traceConnected = false,
        bool traceDisconnected = false,
        bool traceInTrafic = false,
        bool traceOutTrafic = false) {
        Listen = listen ?? throw new ArgumentNullException(nameof(listen));
        ConnectionLimit = connectionLimit;
        IdleTimout = idleTimout;
        DisconnectionTimout = disconnectionTimout;
        TraceConnected = traceConnected;
        TraceDisconnected = traceDisconnected;
        TraceInTrafic = traceInTrafic;
        TraceOutTrafic = traceOutTrafic;
    }

    public int[] Listen { get; }
    public long ConnectionLimit { get; }
    public TimeSpan IdleTimout { get; }
    public TimeSpan DisconnectionTimout { get; }
    public bool TraceConnected { get; }
    public bool TraceDisconnected { get; }
    public bool TraceInTrafic { get; }
    public bool TraceOutTrafic { get; }
}
