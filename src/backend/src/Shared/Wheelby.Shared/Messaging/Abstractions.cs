using Wheelby.Shared.Results;

namespace Wheelby.Shared.Messaging;

/// <summary>
/// Marker for a CQRS request. Convention: every request returns <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="TResponse">The response type, always <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public interface IRequest<TResponse>;

/// <summary>Marker for a command request (intent to change state).</summary>
/// <typeparam name="TResponse">The response type, always <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>;

/// <summary>Marker for a query request (no state change).</summary>
/// <typeparam name="TResponse">The response type, always <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>;

/// <summary>Handles a request of type <typeparamref name="TRequest"/>.</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type, always <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>Handles the request.</summary>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>Represents the next link in the pipeline.</summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public delegate Task<TResponse> HandlerDelegate<TResponse>();

/// <summary>Pipeline step wrapping request handling (validation, authorization, transactions, ...).</summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>Handles the request or delegates to the next link in the pipeline.</summary>
    Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>Validates a request instance. An empty list means the input is valid.</summary>
/// <typeparam name="T">The request type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>Validates the instance and returns the list of error messages (empty when valid).</summary>
    IReadOnlyList<string> Validate(T instance);
}

/// <summary>Dispatches requests to their handler through the registered pipeline behaviors.</summary>
public interface IDispatcher
{
    /// <summary>Sends the request to its handler.</summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}
