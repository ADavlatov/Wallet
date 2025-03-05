using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Infrastructure.Helpers;

public static class TokenHelper
{
    public static AuthDto CreateTokensPair(IOptions<JwtOptions> options, string userId)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var accessToken = jwtSecurityTokenHandler.WriteToken(CreateJwtToken(options, "access", userId));
        var refreshToken = jwtSecurityTokenHandler.WriteToken(CreateJwtToken(options, "refresh", userId));

        if (accessToken is null || refreshToken is null) throw new Domain.Exceptions.AuthenticationException();

        return new AuthDto(accessToken, refreshToken, userId);
    }

    private static JwtSecurityToken CreateJwtToken(IOptions<JwtOptions> options, string type, string userId)
    {
        return new(issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: GetClaims(userId).Claims,
            expires: DateTime.UtcNow.AddDays(type == "access"
                ? options.Value.AccessTokenLifeTimeFromDays
                : options.Value.RefreshTokenLifeTimeFromDays),
            signingCredentials: new(options.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    }

    private static ClaimsIdentity GetClaims(string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userId)
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
}