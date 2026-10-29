namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// Settings fot the data is created when the API starts.
/// </summary>
public sealed class IdentitySeedOptions
{
    public const string SectionName = "Identity";
    /// <summary>When true (the default), the roles are created at startup if they do not exist.</summary>
    public bool SeedOnStartup { get; set; } = true;
}