namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class DemoUserOptions
{
    public const string SectionName = "DemoUser";

    public string Username { get; init; } = "architect";

    public string Password { get; init; } = "ChangeMe123!";

    public string[] Roles { get; init; } = ["Administrator"];
}
