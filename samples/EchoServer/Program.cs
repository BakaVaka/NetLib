using System.Buffers;

using BakaVaka.NetLib.Abstractions;
using BakaVaka.NetLib.Server;


var settings = new TcpServerSettings(new[]{ 8888 }, -1, new DefaultClock(), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5));
var server = new TcpServer(settings, RunEcho);


await server.StartAsync();


Console.WriteLine($"Press [Enter] to complete...");

Console.ReadLine();

await server.StopAsync();


async Task RunEcho(IConnection connection, CancellationToken cancellationToken = default) {

    Console.WriteLine($"{connection.Id} is accepted");
    while( !cancellationToken.IsCancellationRequested ) {

        var reader = connection.Transport.In;
        var writer = connection.Transport.Out;

        var readResult = await reader.ReadAsync(cancellationToken);
        var buffer = readResult.Buffer;

        if( readResult.IsCompleted || readResult.IsCompleted ) {
            break;
        }

        await writer.WriteAsync(buffer.ToArray());

        reader.AdvanceTo(buffer.End);
    }
}