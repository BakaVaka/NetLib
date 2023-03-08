using System;
using System.Net;

namespace BakaVaka.TcpServerLib;

public interface IConnection : IDisposable
{
    public Guid Id { get; }
    public DateTimeOffset? OpenedAt { get; }
    public EndPoint? LocalEndPoint { get; }
    public EndPoint? RemoteEndPoint { get; }
    public void Start();
    public void Abort(Exception reason = null);
    ITransport Transport { get; }
}