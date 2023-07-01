using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib.Utils;
public interface IConnectionTracer {
    public void OnConnected(IConnection connection);
    public void OnDisconnected(IConnection connection);
    public void OnDataReceived(IConnection connection, ReadOnlyMemory<byte> data);
    public void OnDataSent(IConnection connection, ReadOnlyMemory<byte> data);
}
