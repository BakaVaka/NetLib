namespace EchoServer
{
    using BakaVaka.TcpServerLib;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class MyEchoServer : TcpServer<RawMessage, EchoProtocol, EmptyContext>
    {
        public MyEchoServer(IPEndPoint endPoint, 
            Func<Socket, IConnection<RawMessage, EchoProtocol, EmptyContext>> connectionFactory, 
            Func<EmptyContext, RawMessage, Task> messageHandler, 
            int maxConnection) : base(endPoint, connectionFactory, messageHandler, maxConnection)
        {
        }
    }

    public class EchoProtocol : IProtocol<RawMessage, EmptyContext>
    {
        public async Task<RawMessage> Decode(Stream inputStream, EmptyContext context)
        {
            var byteBuffer = new byte[1024];
            var len =  await inputStream.ReadAsync(byteBuffer);
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

    public class EmptyContext : IConnectionContext
    {
        private object _owner;
        public object Owner => _owner;

        public void Bind(object connection)
        {
            if(_owner is null)
            {
                _owner = connection;
                return;
            }
            throw new InvalidOperationException("Already binded");
        }

        public void Dispose(){}

        public T Get<T>() { throw new NotImplementedException(); }
    }

    public class RawMessage
    {
        public byte[] Buffer { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {

            MyEchoServer server = new MyEchoServer(
                new IPEndPoint(0, 8080),
                (s) => new TcpSocketConnection<RawMessage, EchoProtocol, EmptyContext>(s),
                MyMessageHandler,
                1000000);
            await server.Run();
        }

        private static Task MyMessageHandler(EmptyContext context, RawMessage mesage)
        {
            if(context.Owner is TcpSocketConnection< RawMessage, EchoProtocol, EmptyContext> connection)
            {
                if (mesage.Buffer.Length == 0)
                {
                    connection.Close();
                }
                Console.WriteLine($"From connection {connection.ID} : {mesage.Buffer.Length} bytes");
                return connection.Send(mesage);
            }
            return Task.CompletedTask;
        }
    }
}
