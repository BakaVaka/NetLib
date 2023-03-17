namespace BakaVaka.NetLib.Abstractions;

public interface IClock {
    public DateTimeOffset Now { get; }
    public DateTimeOffset NowUTC { get; }
}