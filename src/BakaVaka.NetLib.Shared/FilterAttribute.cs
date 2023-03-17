namespace BakaVaka.NetLib.Shared;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class FilterAttribute<TMessage, TContext> : Attribute {
    public FilterAttribute(Type filterType) {
        if( filterType.IsAssignableTo(typeof(IFilter<TMessage, TContext>)) ) {
            throw new ArgumentException("Filter type should be IFliter<TContex,TMessage>");
        }

        Filter = Activator.CreateInstance(filterType) as IFilter<TMessage, TContext>;
    }

    public IFilter<TMessage, TContext> Filter { get; }
}