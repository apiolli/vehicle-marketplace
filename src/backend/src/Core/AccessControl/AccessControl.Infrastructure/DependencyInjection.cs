using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccessControl(this IServiceCollection services)
    {

        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        // services.AddValidatorsFromAssembly(applicationAssembly, includeInternalTypes: true);

        return services;
    }
}