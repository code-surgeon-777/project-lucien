namespace ProjectLucien.Domain.ValueObjects;

/// <summary>
/// Represents an API key for tenant authentication.
/// Immutable value object that stores the hashed key and metadata.
/// </summary>
public sealed class ApiKey : IEquatable<ApiKey>
{
    public Guid Id { get; }
    public string Name { get; }
    public string KeyHash { get; }
    public DateTime CreatedAt { get; }
    public DateTime? ExpiresAt { get; }
    public bool IsRevoked { get; }

    private ApiKey(Guid id, string name, string keyHash, DateTime createdAt, DateTime? expiresAt, bool isRevoked)
    {
        Id = id;
        Name = name;
        KeyHash = keyHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = isRevoked;
    }

    /// <summary>
    /// Factory method to create a new API key.
    /// </summary>
    public static ApiKey Create(string name, string keyHash, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("API key name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(keyHash))
            throw new ArgumentException("API key hash cannot be empty", nameof(keyHash));

        return new ApiKey(
            Guid.NewGuid(),
            name.Trim(),
            keyHash,
            DateTime.UtcNow,
            expiresAt,
            isRevoked: false
        );
    }

    /// <summary>
    /// Factory method to create an ApiKey from existing data (e.g., from persistence).
    /// </summary>
    public static ApiKey FromExisting(Guid id, string name, string keyHash, DateTime createdAt, DateTime? expiresAt, bool isRevoked)
    {
        return new ApiKey(id, name, keyHash, createdAt, expiresAt, isRevoked);
    }

    /// <summary>
    /// Creates a revoked version of this API key.
    /// </summary>
    public ApiKey Revoke()
    {
        return new ApiKey(Id, Name, KeyHash, CreatedAt, ExpiresAt, isRevoked: true);
    }

    /// <summary>
    /// Validates whether the API key is currently valid for use.
    /// Returns false if the key is revoked or has expired.
    /// </summary>
    public bool IsValid()
    {
        if (IsRevoked)
            return false;

        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
            return false;

        return true;
    }

    public bool Equals(ApiKey? other)
    {
        if (other is null) return false;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => obj is ApiKey other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"ApiKey({Name}, Revoked: {IsRevoked})";
}
