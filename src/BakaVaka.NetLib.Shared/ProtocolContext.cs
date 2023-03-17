using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.TcpServerLib;

/// <summary>
/// 
/// </summary>
public class ProtocolContext<TProtocol, TMessage, TContext>
    where TContext : ProtocolContext<TProtocol, TMessage, TContext>
    where TProtocol : IProtocol<TMessage, TContext>, new() {
    private static readonly TProtocol _protocol = new();
    public IConnection Connection { get; set; }
    public async Task<TMessage> Recieve(CancellationToken cancellationToken = default) {
        return await _protocol.Receive(Connection.Transport.In, (TContext)this, cancellationToken);
    }
    public async Task Send(TMessage message, CancellationToken cancellationToken = default) {
        await _protocol.Send(Connection.Transport.Out, message, (TContext)this, cancellationToken);
    }

}
