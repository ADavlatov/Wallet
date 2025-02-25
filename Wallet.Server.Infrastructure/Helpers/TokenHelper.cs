using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Wallet.Server.Infrastructure.Helpers;

public static class TokenHelper
{
    public static JwtSecurityToken GetJwtToken(string username, Guid userId, int lifetime)
    {
        return new(issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: GetClaims(username, userId.ToString()).Claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(lifetime)),
            signingCredentials: new(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    }
    
    private static ClaimsIdentity GetClaims(string username, string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userId),
            new(ClaimTypes.Name, username)
        };
        
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
    
    private class AuthOptions
    {
        public const string Issuer = "Server";

        public const string Audience = "Client";

        const string Key = "mysupersecret_secretkey!123";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
    }
}