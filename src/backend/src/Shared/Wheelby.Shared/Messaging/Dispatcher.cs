using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Wheelby.Shared.Messaging;

/// <summary>
/// Default <see cref="IDispatcher"/> implementation (RD-02).
/// Resolves the concrete handler for the request type and runs the registered
/// <see cref="IPipelineBehavior{TRequest, TResponse}"/> chain around it.
/// The first registered behavior is the outermost link; the handler is the innermost.
/// Reflection artifacts are built once per request type and kept in a static cache.
/// </summary>
internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private static readonly ConcurrentDictionary<Type, object> Invokers = new();

    /// <inheritdoc />
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var invoker = (Func<IServiceProvider, object, CancellationToken, Task<TResponse>>)Invokers.GetOrAdd(
            request.GetType(),
            static requestType => BuildInvoker<TResponse>(requestType));

        return invoker(serviceProvider, request, cancellationToken);
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<TResponse>> BuildInvoker<TResponse>(Type requestType)
    {
        Type responseType = typeof(TResponse);
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        Type behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        Type behaviorsEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviorType);

        MethodInfo handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;
        MethodInfo behaviorHandleMethod = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;

        return (serviceProvider, request, cancellationToken) =>
        {
            object handler = serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException($"No handler registered for request {requestType.Name}.");

            object? behaviorsObj = serviceProvider.GetService(behaviorsEnumerableType);
            List<object> behaviors = behaviorsObj is IEnumerable enumerable
                ? enumerable.Cast<object>().ToList()
                : [];

            HandlerDelegate<TResponse> pipeline = () =>
                (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;

            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                object behavior = behaviors[i];
                HandlerDelegate<TResponse> next = pipeline;
                pipeline = () =>
                    (Task<TResponse>)behaviorHandleMethod.Invoke(behavior, [request, next, cancellationToken])!;
            }

            return pipeline();
        };
    }
}
