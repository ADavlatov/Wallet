using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Wallet.Server.Infrastructure.Helpers;

public static class TokenHelper
{
    public static JwtSecurityToken GetJwtToken(string userId, int lifetime)
    {
        return new(issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: GetClaims(userId).Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(lifetime)),
            signingCredentials: new(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
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

    private class AuthOptions
    {
        public const string Issuer = "Server.qwe";

        public const string Audience = "Client.qwe";

        const string Key = "DjWu65sGJeM4Vr1i/4Zc3dCY3JCiwQTOiU6F1uG02SsQ5SoAycUgGBRFhpGW/dQu";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
    }
}