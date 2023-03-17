namespace BakaVaka.NetLib.Shared;

public interface IRouteMatcher<TMessage, TContext> {
    public bool IsMatch(TMessage message, TContext context);
}


