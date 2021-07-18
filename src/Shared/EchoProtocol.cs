namespace Shared
{
    using BakaVaka.TcpServerLib;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class EchoProtocol : IProtocol<RawMessage, EmptyContext>
    {
        public async Task<RawMessage> Decode(Stream inputStream, EmptyContext context, CancellationToken cancellationToken)
        {
            var byteBuffer = new byte[1024];
            int len = await inputStream.ReadAsync(byteBuffer, cancellationToken);
            if (len <= 1)
            {
                throw new IOException("Invalid message");
            }
            return new RawMessage
            {
                Buffer = byteBuffer[..len]
            };
        }

        public byte[] Encode(RawMessage message, EmptyContext clientContext)
        {
            return message.Buffer ?? Array.Empty<byte>();
        }
    }

}
