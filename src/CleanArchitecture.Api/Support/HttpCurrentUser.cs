using System.Security.Claims;
using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Api.Support;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? "anonymous";

    public IReadOnlyCollection<string> Roles =>
        _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToArray()
        ?? [];

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
