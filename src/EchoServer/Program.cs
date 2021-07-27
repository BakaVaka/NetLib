namespace EchoServer
{
    using BakaVaka.TcpServerLib;
    using Microsoft.Extensions.Logging;
    using Shared;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class MyEchoServer : TcpServer<RawMessage, EchoProtocol>
    {
        public MyEchoServer(
            ServerSettings settings,
            ILogger<MyEchoServer> logger)
            : base(settings,
                  logger,
                  (socket) => new TcpSocketConnection<RawMessage, EchoProtocol>(socket, EchoHandler.Instance))
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
            EchoHandler.Instance.Logger = logger;
            var echoServer = new MyEchoServer
                (new ServerSettings
                (new IPEndPoint(0, 8080), 10000000,
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(100),
                    TimeSpan.FromSeconds(10)),
                logger);

            await echoServer.Start();
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
