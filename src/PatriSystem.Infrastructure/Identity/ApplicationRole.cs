using Microsoft.AspNetCore.Identity;

namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// System role (Admin, Cajero). Extends ASP.NET Core Identity with <see cref="Guid"/> key.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
}