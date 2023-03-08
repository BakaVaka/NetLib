using System;
using System.Net;
using System.Threading.Tasks;

using BakaVaka.TcpServerLib;

using Microsoft.Extensions.Logging;

using Shared;

namespace EchoServer
{
    public class MyEchoServer : TcpServer
    {
        public MyEchoServer(
            ServerSettings settings,
            ILogger<MyEchoServer> logger)
            : base(settings,
                  logger,
                  (socket) => new TcpSocketConnection(socket))
        {
        }

    }

    public class EchoHandler : IMessageHandler<RawMessage, EchoProtocol>
    {
        private static EchoHandler _instance = new();
        public static EchoHandler Instance => _instance;
        public ILogger Logger { get; set; }
        private EchoHandler() { }
        public Task HandleMessage(RawMessage message, IConnection<RawMessage, EchoProtocol> connection)
        {
            Logger?.LogTrace($"Message received. Len: {message.Buffer.Length}");
            return connection.Send(message);
        }
    }

    class Program
    {
        static async Task Main()
        {

            ILogger<MyEchoServer> logger = new ConsoleLogger<MyEchoServer>();
            var settings = new ServerSettings(
                ListeningEndPoint: new IPEndPoint(0, 8080),
                MaxConnections: int.MaxValue,
                DisconnectTimeout: TimeSpan.FromSeconds(120),
                HeartbeatTimeout: TimeSpan.FromSeconds(1),
                IdleTimeout: TimeSpan.FromSeconds(10));

            EchoHandler.Instance.Logger = logger;
            var echoServer = new MyEchoServer(settings, logger);
            var run = echoServer.Run();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            echoServer.Stop();
            await run;
        }
        private class ConsoleLogger<T> : ILogger<T>
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"[{DateTime.Now}][{logLevel}] {state}");
            }
        }

    }
}
