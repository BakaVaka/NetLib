namespace BakaVaka.NetLib.Shared;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class RouteAttribute<TMessage, TContext> : Attribute {
    private IRouteMatcher<TMessage, TContext>? _routeMatcher;
    public RouteAttribute(Type matcher) {
        _routeMatcher = (IRouteMatcher<TMessage, TContext>)Activator.CreateInstance(matcher);
    }
    public bool IsMatch(TMessage message, TContext context)
        => _routeMatcher.IsMatch(message, context);
}


