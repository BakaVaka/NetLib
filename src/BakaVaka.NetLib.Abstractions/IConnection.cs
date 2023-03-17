namespace BakaVaka.NetLib.Abstractions;

public interface IConnection : IDisposable {
    public Guid Id { get; }
    public DateTimeOffset? StartedAt { get; }
    public void Start();
    public void Abort(Exception reason = null);
    public ITransport Transport { get; }
    public IItemStore Items { get; }
    public IFeatureCollection Features { get; }
}