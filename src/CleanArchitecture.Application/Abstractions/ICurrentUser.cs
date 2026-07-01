namespace CleanArchitecture.Application.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsInRole(string role);
}
