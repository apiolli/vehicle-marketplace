using AccessControl.Application.Ping;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccessControl(this IServiceCollection services)
    {
        var applicationAssembly = typeof(PingQuery).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

        return services;
    }
}