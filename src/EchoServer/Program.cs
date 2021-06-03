namespace EchoServer
{
    using BakaVaka.TcpServerLib;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new TcpServer(
                new IPEndPoint(IPAddress.Any, 80),
                EchoHandler,
                2);
            server.Started += () => Console.WriteLine("Server started");
            server.Stopped += () => Console.WriteLine("Server stopped");
            server.Start();
            Console.WriteLine("Press ESC to stop server");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            server.Stop();
            Console.ReadKey();
        }

        private static async Task EchoHandler(ITransportConnection connection, CancellationToken stopToken)
        {
            byte[] buffer = new byte[1024];
            Console.WriteLine($"Start handel connection form {connection.RemoteEndPoint}");
            while (!stopToken.IsCancellationRequested)
            {
                var bytes = await connection.Receve(buffer);
                if (bytes == 0)
                {
                    break;
                }
                    await connection.Send(buffer[..bytes]);
            }
            Console.WriteLine("Connection complete");
        }
    }
}
