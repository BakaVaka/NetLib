using System.IO.Pipelines;

namespace BakaVaka.NetLib.Abstractions;

/// <summary>
/// Абстрактный протокол, например HTTP, IRC, SNMP, etc
/// </summary>
public interface IProtocol<TMessage, TContext> {
    public ValueTask<TMessage> Receive(PipeReader reader, TContext context, CancellationToken cancellationToken = default);
    public ValueTask Send(PipeWriter writer, TMessage message, TContext context, CancellationToken cancellationToken = default);
}
/// <summary>
/// Протокол без контекста (для простых протоколов, не содержащих какой-то контекстуальной логики)
/// </summary>
public interface IProtocol<TMessage> {
    public ValueTask<TMessage> Receive(PipeReader reader, CancellationToken cancellationToken = default);
    public ValueTask Send(PipeWriter writer, TMessage message, CancellationToken cancellationToken = default);
}