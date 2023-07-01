using System.Buffers;
using System.IO.Pipelines;

using BakaVaka.NetLib.Abstractions;

namespace Shared;
public class EchoProtocol : ICodec<RawMessage> {
    public async Task<RawMessage> Decode(Stream inputStream, IConnection connection, CancellationToken cancellationToken) {
        var byteBuffer = new byte[1024];
        int len = await inputStream.ReadAsync(byteBuffer, cancellationToken);
        return len <= 1
            ? throw new IOException("Invalid message")
            : new RawMessage {
                Buffer = byteBuffer[..len]
            };
    }

    public byte[] Encode(RawMessage message, IConnection connection) {
        return message.Buffer ?? Array.Empty<byte>();
    }

    public async ValueTask<RawMessage> Decode(PipeReader reader, CancellationToken cancellationToken = default) {
        var readResult = await reader.ReadAsync(cancellationToken);
        var message = readResult.Buffer.ToArray();
        reader.AdvanceTo(readResult.Buffer.End, readResult.Buffer.End);

        return new RawMessage() { Buffer = message };
    }

    public async ValueTask Encode(PipeWriter writer, RawMessage message, CancellationToken cancellationToken = default) {
        await writer.WriteAsync(message.Buffer, cancellationToken); ;
    }
}
