

using System.IO.Pipelines;
using System.Net.Sockets;


namespace BakaVaka.TcpServerLib;
internal sealed class SocketConnectionReceiver {
    private readonly Socket _socket;
    private readonly PipeWriter _writer;
    public SocketConnectionReceiver(
        Socket socket,
        PipeWriter writer
        ) {
        _socket = socket;
        _writer = writer;
    }
    public async Task Receive(CancellationToken cancellationToken) {
        while( !cancellationToken.IsCancellationRequested ) {
            var buffer = _writer.GetMemory(1024);
            var receivedBytes = await _socket.ReceiveAsync(buffer, cancellationToken);
            if( receivedBytes == 0 ) {
                break;
            }
            var receivedData = buffer.Slice(0, receivedBytes);
            await _writer.WriteAsync(buffer, cancellationToken);
        }
    }
}
