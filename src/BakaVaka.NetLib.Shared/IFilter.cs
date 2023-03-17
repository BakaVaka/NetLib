namespace BakaVaka.NetLib.Shared;


public interface IFilter<TMessage, TContext> {
    public bool CanGoForward(TMessage message, TContext context);
    public Task OnFail(TMessage message, TContext context, CancellationToken cancellationToken);
}
