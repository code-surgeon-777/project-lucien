namespace ProjectLucien.Infrastructure.Auth0;

/// <summary>
/// Configuration settings for Auth0 integration.
/// </summary>
public class Auth0Config
{
    /// <summary>
    /// The Auth0 domain (e.g., your-tenant.auth0.com).
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// The Auth0 client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Auth0 client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The Auth0 audience (API identifier).
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}
