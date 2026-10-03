using FluentValidation;
using MediatR;
using SharedKernel.Exceptions;

namespace Shared.Application;

internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorList = validators.ToList();
        if (validatorList.Count == 0)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var messages = new List<string>();

        foreach (var validator in validatorList)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            messages.AddRange(result.Errors
                .Select(failure => failure.ErrorMessage)
                .Where(message => !string.IsNullOrWhiteSpace(message)));
        }

        if (messages.Count == 0)
        {
            return await next();
        }

        throw new ValidationException(string.Join(" ", messages.Distinct()));
    }
}