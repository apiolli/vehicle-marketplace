using System.Reflection;
using Wheelby.Shared.Results;

namespace Wheelby.Shared.Messaging;

/// <summary>
/// Pipeline behavior running all registered <see cref="IValidator{T}"/> before the handler (RD-07).
/// When validation fails the handler is skipped and a failed result of the same
/// <typeparamref name="TResponse"/> type is returned, carrying a <see cref="ErrorType.Validation"/>
/// error. Works with both <see cref="Result"/> and <see cref="Result{T}"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type, always <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    /// <inheritdoc />
    public Task<TResponse> Handle(
        TRequest request,
        HandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        List<string> failures = [];
        foreach (IValidator<TRequest> validator in validators)
        {
            IReadOnlyList<string> messages = validator.Validate(request);
            foreach (string message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    failures.Add(message);
                }
            }
        }

        if (failures.Count == 0)
        {
            return next();
        }

        Error error = Error.Validation("validation.failed", string.Join(" ", failures));
        return Task.FromResult(CreateFailure(error));
    }

    private static TResponse CreateFailure(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo failureMethod = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(Result<object>.Failure), BindingFlags.Public | BindingFlags.Static)!;
            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior only supports {nameof(Result)} or {nameof(Result<int>)} responses, but got {typeof(TResponse).Name}.");
    }
}
