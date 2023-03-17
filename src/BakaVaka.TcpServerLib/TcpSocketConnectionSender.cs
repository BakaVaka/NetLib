using System.IO.Pipelines;
using System.Net.Sockets;

namespace BakaVaka.TcpServerLib;

internal sealed class TcpSocketConnectionSender {
    private readonly Socket _socket;
    private readonly PipeReader _reader;
    public TcpSocketConnectionSender(Socket socket, PipeReader reader) {
        _socket = socket;
        _reader = reader;
    }

    public async Task Send(CancellationToken cancellationToken) {
        while( !cancellationToken.IsCancellationRequested ) {
            try {
                var readResult = await _reader.ReadAsync(cancellationToken);
                var buffer = readResult.Buffer;
                if( buffer.IsSingleSegment ) {
                    await _socket.SendAsync(buffer.First, cancellationToken);
                }
                else {
                    foreach( var segment in buffer ) {
                        await _socket.SendAsync(segment, cancellationToken);
                    }
                }
                _reader.AdvanceTo(buffer.End, buffer.End);
            }
            catch( Exception ex ) {
                break;
            }

        }
    }
}
