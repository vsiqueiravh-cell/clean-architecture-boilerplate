using CleanArchitecture.Application.Authentication;

namespace CleanArchitecture.Application.Abstractions;

public interface IJwtTokenService
{
    AccessTokenResponse CreateToken(LoginRequest request);
}
