namespace BakaVaka.TcpServerLib.Features;
public interface IHeartbeatFeature {
    public void OnHeartbeat(DateTimeOffset now);
}
