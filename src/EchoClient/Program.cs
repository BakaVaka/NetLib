namespace EchoClient
{
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using BakaVaka.TcpServerLib;

    using Shared;

    class Program
    {
        static async Task Main()
        {
            var range = Enumerable.Range(0, 100).Select(x => new Thread(async (_) => await DDOSS_Test()));
            foreach (var t in range)
                t.Start();
            await Task.Delay(-1);
        }
        internal class EchoHandler : IMessageHandler<RawMessage>
        {
            private static readonly EchoHandler _instance = new();
            private EchoHandler() { }
            public static EchoHandler Instance => _instance;
            public Task HandleMessage(RawMessage message, IConnection connection, CancellationToken cancellationToken)
            {
                return connection.Send(message);
            }
        }

        public class EchoConnection : TcpSocketConnection<RawMessage, EchoProtocol>
        {
            public int SendCount { get; private set; } = 0;
            public EchoConnection(Socket cleintSocket, IMessageHandler<RawMessage, EchoProtocol> messageHandler) : base(cleintSocket, messageHandler)
            {
            }
            public override Task Send(RawMessage message)
            {
                SendCount++;
                return base.Send(message);
            }
        }

        private static async Task DDOSS_Test()
        {
            try
            {
                await Task.Delay(1);
                Socket clientSocket = new(SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect("localhost", 8080);
                var messageToSend = new RawMessage
                {
                    Buffer = Encoding.ASCII.GetBytes("Hello\r\n")
                };
                EchoConnection connection = new(clientSocket, EchoHandler.Instance);
                connection.Closed += (s, e) =>
                {
                    Console.WriteLine($"Connection closed");

                };

                connection.Open();
                await connection.Send(messageToSend).ConfigureAwait(true);
                while (connection.SendCount < 10)
                {
                    await Task.Delay(1);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
