using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// Creates the system roles at startup. It is idempotent: a role that already exists is left untouched.
/// </summary>
internal sealed partial class IdentitySeedHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<IdentitySeedOptions> options,
    ILogger<IdentitySeedHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.SeedOnStartup)
        {
            LogSeedSkipped(logger);
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var roleName in RoleNames.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create role '{roleName}': {errors}");
            }

            LogRoleCreated(logger, roleName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [LoggerMessage(Level = LogLevel.Information, Message = "Identity role {RoleName} created")]
    private static partial void LogRoleCreated(ILogger logger, string roleName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Identity seed skipped (Identity:SeedOnStartup is false)")]
    private static partial void LogSeedSkipped(ILogger logger);
}