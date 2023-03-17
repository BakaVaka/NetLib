namespace BakaVaka.NetLib.Shared;

public delegate bool RouteMatcher<TMessage, TContext>(TMessage message, TContext context);
public delegate Task MessageHandler<TMessage, TContext>(TMessage message, TContext context, CancellationToken cancellationToken = default);

public class RouteDescriptor<TMessage, TContext> {

    public RouteMatcher<TMessage, TContext> Matcher { get; set; }
    public MessageHandler<TMessage, TContext> Handler { get; set; }
    public IEnumerable<IFilter<TMessage, TContext>> Filters { get; set; } = Array.Empty<IFilter<TMessage, TContext>>();
}


