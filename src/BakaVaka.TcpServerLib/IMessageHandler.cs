using System.Threading;
using System.Threading.Tasks;

namespace BakaVaka.TcpServerLib;

public interface IMessageHandler<TMessage, TContext>
{
    public Task HandleMessage(
        TMessage message,
        TContext context,
        IConnection connection,
        CancellationToken cancellation = default);
}
public interface IMessageHandler<TMessage>
{
    public Task HandleMessage(
        TMessage message,
        IConnection connection,
        CancellationToken cancellation = default);
}