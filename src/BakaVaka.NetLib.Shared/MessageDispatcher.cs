using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace BakaVaka.NetLib.Shared;

public class MessageDispatcher<TMessage, TContext> {
    private readonly IServiceProvider _serviceProvider;
    private List<RouteDescriptor<TMessage, TContext>> _routes = new();
    public MessageDispatcher(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }
    public void Bind<TController>() where TController : Controller<TMessage, TContext> {
        var routes = typeof(TController)
            .GetMethods()
            .Where(HasPosibleRoutes())
            .Select(GetHandlersFromControllerOf<TController>());
        _routes.AddRange(routes);

        Func<MethodInfo, RouteDescriptor<TMessage, TContext>> GetHandlersFromControllerOf<TController>()
            where TController : Controller<TMessage, TContext> {
            return method => {
                var routeInfo = method.GetCustomAttribute<RouteAttribute<TMessage, TContext>>();
                var filters = method.GetCustomAttributes<FilterAttribute<TMessage, TContext>>();
                return new RouteDescriptor<TMessage, TContext>() {
                    Matcher = routeInfo.IsMatch,
                    Filters = filters.Select(x=>x.Filter).ToArray(),
                    Handler = async (message, context, canellationToken) => {
                        var controller = ActivatorUtilities.CreateInstance<TController>(_serviceProvider);
                        controller.ControllerContext = new ControllerContext<TMessage, TContext>(message, context);
                        var parameters = method.GetParameters()
                            .Select(x => _serviceProvider.GetRequiredService(x.ParameterType))
                            .ToArray();

                        if( method.ReturnType.IsAssignableTo(typeof(Task)) ) {
                            var task = method.Invoke(controller, parameters) as Task;
                            await task;
                        }
                        else {
                            method.Invoke(controller, parameters);
                        }

                        if( controller is IDisposable ) {
                            var disposable = controller as IDisposable;
                            disposable.Dispose();
                        }
                        else if( controller is IAsyncDisposable ) {

                            var disposable = controller as IAsyncDisposable;
                            await disposable.DisposeAsync();
                        }
                    }
                };
            };
        }

        static Func<MethodInfo, Boolean> HasPosibleRoutes()
            => method => !method.IsStatic && method.IsPublic && method.CustomAttributes.Any(
                attr => attr.AttributeType.IsAssignableTo(typeof(RouteAttribute<TMessage, TContext>)));
    }

    public async Task DispatchMessageAsync(TMessage message, TContext context, CancellationToken cancellationToken = default) {
        var route = _routes.FirstOrDefault(x => x.Matcher(message, context));
        if( route is not null ) {
            foreach(var filter in route.Filters ) {
                if( !filter.CanGoForward(message, context) ) {
                    await filter.OnFail(message, context, cancellationToken);
                    return;
                }
            }
            await route.Handler(message, context, cancellationToken);
        }
    }
}


