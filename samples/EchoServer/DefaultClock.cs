using BakaVaka.NetLib.Abstractions;

internal class DefaultClock : IClock {
    public DateTimeOffset Now => DateTimeOffset.Now;
    public DateTimeOffset NowUTC => DateTimeOffset.UtcNow;
}