using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib;
public class TcpServerBuilder
{
    private List<IPEndPoint> _endpoints = new();
    private Func<IConnection, CancellationToken, Task> _connectionHandler;
    private Func<IConnection> _idleHandler;
    public void OnConnected(Func<IConnection, CancellationToken, Task> connectionHandler)
    {
        _connectionHandler = connectionHandler;
    }

    public TcpServer Build()
    {
        return new TcpServer(
            new ServerSettings(_endpoints.ToArray(), -1, TimeSpan.FromMilliseconds);
    }
}
