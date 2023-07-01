using BakaVaka.NetLib.Abstractions;

namespace BakaVaka.NetLib.Shared;

/// <summary>
/// Контекст протокола
/// </summary>
public class ProtocolContext<TProtocol, TMessage, TContext>
    where TContext : ProtocolContext<TProtocol, TMessage, TContext>
    where TProtocol : ICodec<TMessage, TContext>, new() {
    private static readonly TProtocol _protocol = new();
    public IConnection Connection { get; set; }
    public async Task<TMessage> Recieve(CancellationToken cancellationToken = default) {
        return await _protocol.Decode(Connection.Transport.In, (TContext)this, cancellationToken);
    }
    public async Task Send(TMessage message, CancellationToken cancellationToken = default) {
        await _protocol.Encode(Connection.Transport.Out, message, (TContext)this, cancellationToken);
    }
}
