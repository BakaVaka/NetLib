namespace BakaVaka.TcpServerLib
{
    using System;
    using System.Net;

    public record ServerSettings(
        IPEndPoint[] ListeningEndPoint,
        int MaxConnections,
        TimeSpan HeartbeatTimeout,
        TimeSpan DisconnectTimeout,
        TimeSpan IdleTimeout);
}
