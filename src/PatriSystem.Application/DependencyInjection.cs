using Microsoft.Extensions.DependencyInjection;

namespace PatriSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}