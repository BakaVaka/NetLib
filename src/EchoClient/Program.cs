namespace EchoClient
{
    using BakaVaka.TcpServerLib;
    using Shared;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main()
        {
            Socket clientSocket = new(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect("localhost", 8080);
            int sendCount = 10;
            TcpSocketConnection<RawMessage, EchoProtocol, EmptyContext> connection = new();
            connection.Closed += (s,e) =>
            {
                Console.WriteLine($"Connection closed");

            };

            connection.OnMessage(async (message) =>
            {

                if(sendCount > 0)
                {
                    Interlocked.Decrement(ref sendCount);
                    await connection.Send(message);
                    return;
                }
                connection.Close();
            });

            connection.BindSocket(clientSocket);

            await connection.Send(new RawMessage
            {
                Buffer = Encoding.ASCII.GetBytes("Hello")
            });
            while(sendCount>0)
            {
                await Task.Delay(-1);
            }
            Console.WriteLine("Application complete");
        }
    }
}
