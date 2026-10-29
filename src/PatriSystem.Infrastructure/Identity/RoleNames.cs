namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// Names of the roles that exist in the system.
/// </summary>
public static class RoleNames
{
    public const string Admin = "Admin";

    public const string Cashier = "Cajero";

    public static readonly IReadOnlyList<string> All = [Admin, Cashier];
}