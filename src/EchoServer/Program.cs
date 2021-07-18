namespace EchoServer
{
    using BakaVaka.TcpServerLib;
    using Microsoft.Extensions.Logging;
    using Shared;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class MyEchoServer : TcpServer<RawMessage, EchoProtocol, EmptyContext>
    {
        public MyEchoServer(
            ServerSettings settings, 
            ILogger<MyEchoServer> logger)
            : base(settings, 
                  logger,
                  new EmptyDispatcher(),
                  () => new TcpSocketConnection<RawMessage, EchoProtocol, EmptyContext>())
        {
        }

    }

    public class EmptyDispatcher : MessageDispatcherBase<RawMessage, EmptyContext>
    {
        protected override Task Dispatch<TProtocol>(
            RawMessage message, 
            IConnection<RawMessage, TProtocol, EmptyContext> connection, 
            EmptyContext contex)
        {
            Console.WriteLine("Message accepted");
            return connection.Send(message);
        }
    }   

    class Program
    {
        static async Task Main()
        {

            ILogger<MyEchoServer> logger = new ConsoleLogger<MyEchoServer>();

            var echoServer = new MyEchoServer
                (new ServerSettings
                (new IPEndPoint(0, 8080), 1000000,
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
