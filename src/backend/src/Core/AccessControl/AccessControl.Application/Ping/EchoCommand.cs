using FluentValidation;
using MediatR;
using Shared.Application.Abstractions;

namespace AccessControl.Application.Ping;

public sealed record EchoCommand(string Text) : ICommand<string>;

internal sealed class EchoCommandValidator : AbstractValidator<EchoCommand>
{
    public EchoCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("El texto es obligatorio.")
            .MaximumLength(10).WithMessage("El texto no puede pasar de 10 caracteres.");
    }
}

internal sealed class EchoCommandHandler : IRequestHandler<EchoCommand, string>
{
    public Task<string> Handle(EchoCommand request, CancellationToken cancellationToken)
        => Task.FromResult($"echo: {request.Text}");
}