using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Application;

public static class DependencyInjection
{
    /// <summary>Call exactly once, from the Host. Registers the pipeline behavior and the clock.</summary>
    public static IServiceCollection AddSharedApplication(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}