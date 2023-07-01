using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib.Events;

public class ConnectionClosedEventArgs : EventArgs {
    public ConnectionClosedEventArgs(IConnection connection) {
        Connection = connection;
    }

    public IConnection Connection { get; set; }
}