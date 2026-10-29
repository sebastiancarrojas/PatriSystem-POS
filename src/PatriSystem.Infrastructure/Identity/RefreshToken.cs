namespace PatriSystem.Infrastructure.Identity;

/// <summary>
/// Refresh token issued to a user. Only the hash of the token is stored; the raw value is never persisted.
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    /// <summary>SHA-256 of the raw token, as 64 hexadecimal characters.</summary>
    public required string TokenHash { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
    /// <summary>Null while the token is still valid.</summary>
    public DateTime? RevokedAtUtc { get; set; }
}