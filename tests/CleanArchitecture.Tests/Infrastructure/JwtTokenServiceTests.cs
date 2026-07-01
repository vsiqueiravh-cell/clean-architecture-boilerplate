using CleanArchitecture.Application.Authentication;
using CleanArchitecture.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Tests.Infrastructure;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ReturnsBearerTokenForValidDemoUser()
    {
        var service = new JwtTokenService(
            Options.Create(new DemoUserOptions
            {
                Username = "architect",
                Password = "ChangeMe123!",
                Roles = ["Administrator"]
            }),
            Options.Create(new JwtOptions
            {
                Issuer = "clean-architecture-boilerplate",
                Audience = "clean-architecture-api",
                SigningKey = "local-development-signing-key-change-before-production"
            }));

        var token = service.CreateToken(new LoginRequest("architect", "ChangeMe123!"));

        Assert.Equal("Bearer", token.TokenType);
        Assert.Contains("Administrator", token.Roles);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
    }
}
