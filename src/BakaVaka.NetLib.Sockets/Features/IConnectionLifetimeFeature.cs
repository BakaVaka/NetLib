namespace BakaVaka.TcpServerLib.Features;

public interface IConnectionLifetimeFeature {
    public DateTimeOffset? StartedAt { get; }
    public DateTimeOffset? LastReceivedPacket { get; }
    public DateTimeOffset? LastSentPacket { get; }

}
