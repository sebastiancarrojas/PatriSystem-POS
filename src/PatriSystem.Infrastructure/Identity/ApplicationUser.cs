using Microsoft.AspNetCore.Identity;

namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// System user account. Extends ASP.NET Core Identity with <see cref="Guid"/> key.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
}