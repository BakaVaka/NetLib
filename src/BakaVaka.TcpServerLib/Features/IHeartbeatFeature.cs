namespace BakaVaka.TcpServerLib.Features;
internal interface IHeartbeatFeature {
    public void OnHeartbeat(DateTimeOffset now);
}
public interface IConnectionLifetimeFeature {
    public DateTimeOffset? StartedAt { get; }
    public DateTimeOffset? LastReceivedPacket { get; }
    public DateTimeOffset? LastSentPacket { get; }

}

public interface IConnectionStatisticFeature {
    public ulong SentBytes { get; }
    public ulong ReceivedBytes { get; }
    public void ResetStatistic();
}