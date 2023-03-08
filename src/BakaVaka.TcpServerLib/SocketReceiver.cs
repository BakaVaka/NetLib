using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib;
internal sealed class SocketReceiver
{
    private readonly Socket _socket;
    private readonly PipeWriter _writer;

    public SocketReceiver(
        Socket socket, 
        PipeWriter writer)
    {
        _socket = socket;
        _writer = writer;
    }
    public async Task Receive(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var buffer = _writer.GetMemory(1024);

                var receivedBytes = await _socket.ReceiveAsync(buffer, cancellationToken);
                if(receivedBytes == 0)
                {
                    break;
                }
                await _writer.WriteAsync(buffer, cancellationToken);
            }
            catch(Exception ex)
            {
                break;
            }
        }
    }
}
