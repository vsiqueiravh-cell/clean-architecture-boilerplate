namespace CleanArchitecture.Application.Authentication;

public sealed record AccessTokenResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    IReadOnlyCollection<string> Roles);
