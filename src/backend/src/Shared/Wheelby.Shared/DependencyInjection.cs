using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wheelby.Shared.Messaging;
using Wheelby.Shared.Time;

namespace Wheelby.Shared;

/// <summary>Dependency injection registration for the shared kernel (RD-02, RD-07, RD-11).</summary>
public static class SharedKernelServiceExtensions
{
    /// <summary>
    /// Registers the shared kernel: <see cref="IClock"/> as singleton,
    /// <see cref="IDispatcher"/> as scoped, and the validation behavior as open generic pipeline step.
    /// </summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Scans the assembly and registers every concrete handler (<see cref="IRequestHandler{TRequest, TResponse}"/>)
    /// and validator (<see cref="IValidator{T}"/>) as scoped, including internal types.
    /// Abstract classes and open generics are ignored.
    /// </summary>
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
            {
                continue;
            }

            foreach (Type serviceType in type.GetInterfaces())
            {
                if (!serviceType.IsGenericType)
                {
                    continue;
                }

                Type definition = serviceType.GetGenericTypeDefinition();
                if (definition == typeof(IRequestHandler<,>) || definition == typeof(IValidator<>))
                {
                    services.AddScoped(serviceType, type);
                }
            }
        }

        return services;
    }
}
