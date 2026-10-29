using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using PatriSystem.Infrastructure.Identity;

using Shouldly;

namespace PatriSystem.Api.IntegrationTests;

public sealed class IdentityPolicyTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Theory]
    [InlineData("Corta1A")]
    [InlineData("sinmayuscula123")]
    [InlineData("SINMINUSCULA123")]
    [InlineData("SinNumerosAqui")]
    public async Task ValidatePassword_WeakPassword_IsRejected(string password)
    {
        var result = await ValidateAsync(password);

        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ValidatePassword_StrongPassword_IsAccepted()
    {
        var result = await ValidateAsync("Segura12345");

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void LockoutOptions_Configured_AllowFiveAttemptsAndLockFifteenMinutes()
    {
        using var scope = factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;

        options.Lockout.AllowedForNewUsers.ShouldBeTrue();
        options.Lockout.MaxFailedAccessAttempts.ShouldBe(5);
        options.Lockout.DefaultLockoutTimeSpan.ShouldBe(TimeSpan.FromMinutes(15));
    }

    private async Task<IdentityResult> ValidateAsync(string password)
    {
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = "test@example.com" };

        foreach (var validator in userManager.PasswordValidators)
        {
            var result = await validator.ValidateAsync(userManager, user, password);

            if (!result.Succeeded)
            {
                return result;
            }
        }

        return IdentityResult.Success;
    }
}