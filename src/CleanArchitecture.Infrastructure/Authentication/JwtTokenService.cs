using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Infrastructure.Authentication;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly DemoUserOptions _demoUserOptions;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(IOptions<DemoUserOptions> demoUserOptions, IOptions<JwtOptions> jwtOptions)
    {
        _demoUserOptions = demoUserOptions.Value;
        _jwtOptions = jwtOptions.Value;
    }

    public AccessTokenResponse CreateToken(LoginRequest request)
    {
        if (!IsValidUser(request))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(ClaimTypes.NameIdentifier, request.Username),
            new(ClaimTypes.Name, request.Username)
        };

        claims.AddRange(_demoUserOptions.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new AccessTokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expiresAt,
            _demoUserOptions.Roles);
    }

    private bool IsValidUser(LoginRequest request)
    {
        return string.Equals(request.Username, _demoUserOptions.Username, StringComparison.Ordinal)
            && string.Equals(request.Password, _demoUserOptions.Password, StringComparison.Ordinal);
    }
}
