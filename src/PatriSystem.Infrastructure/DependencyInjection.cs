using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PatriSystem.Infrastructure.Health;
using PatriSystem.Infrastructure.Options;

namespace PatriSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"], timeout: TimeSpan.FromSeconds(5));

        return services;
    }
}