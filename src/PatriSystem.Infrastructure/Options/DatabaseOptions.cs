using System.ComponentModel.DataAnnotations;

namespace PatriSystem.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "Falta la configuración Database:ConnectionString.")]
    public string ConnectionString { get; init; } = string.Empty;
}