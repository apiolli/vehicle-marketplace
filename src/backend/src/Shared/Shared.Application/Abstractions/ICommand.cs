using System;
using MediatR;

namespace Shared.Application.Abstractions;

public interface ICommand : ICommand<Unit>;
public interface ICommand<out TResponse> : IRequest<TResponse>;