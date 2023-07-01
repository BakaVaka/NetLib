using System.IO.Pipelines;

namespace BakaVaka.NetLib.Abstractions;

/// <summary>
/// Абстрактный кодек, например HTTP, IRC, SNMP, etc
/// </summary>
public interface ICodec<TMessage, TContext> {
    public ValueTask<TMessage> Decode(PipeReader reader, TContext context, CancellationToken cancellationToken = default);
    public ValueTask Encode(PipeWriter writer, TMessage message, TContext context, CancellationToken cancellationToken = default);
}
/// <summary>
/// Кодек без контекста (для простых протоколов, не содержащих какой-то контекстуальной логики)
/// </summary>
public interface ICodec<TMessage> {
    public ValueTask<TMessage> Decode(PipeReader reader, CancellationToken cancellationToken = default);
    public ValueTask Encode(PipeWriter writer, TMessage message, CancellationToken cancellationToken = default);
}