namespace BakaVaka.TcpServerLib.Features;

public interface IConnectionStatisticFeature {
    public ulong SentBytes { get; }
    public ulong ReceivedBytes { get; }
    public void ResetStatistic();
}