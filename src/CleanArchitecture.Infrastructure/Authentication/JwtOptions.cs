namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "clean-architecture-boilerplate";

    public string Audience { get; init; } = "clean-architecture-api";

    public string SigningKey { get; init; } = "local-development-signing-key-change-before-production";

    public int ExpirationMinutes { get; init; } = 60;
}
