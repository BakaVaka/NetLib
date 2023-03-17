namespace BakaVaka.NetLib.Shared;

public class ControllerContext<TMessage, TContext> {
    public ControllerContext(TMessage message, TContext context) {
        Message = message;
        Context = context;
    }
    public TMessage Message { get; }
    public TContext Context { get; }
}


