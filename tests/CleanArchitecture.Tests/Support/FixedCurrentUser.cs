using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Tests.Support;

public sealed class FixedCurrentUser : ICurrentUser
{
    public FixedCurrentUser(string userId, IReadOnlyCollection<string> roles)
    {
        UserId = userId;
        Roles = roles;
    }

    public string UserId { get; }

    public IReadOnlyCollection<string> Roles { get; }

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
