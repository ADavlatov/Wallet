using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Infrastructure.Helpers;

public static class TokenHelper
{
    public static AuthDto CreateTokensPair(IOptions<JwtOptions> options, Guid id, ILogger logger)
    {
        logger.LogInformation("Запрос на создание пары токенов для пользователя с ID: {UserId}", id);
        var userId = id.ToString();
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var accessToken = jwtSecurityTokenHandler.WriteToken(CreateJwtToken(options, "access", userId, logger));
        var refreshToken = jwtSecurityTokenHandler.WriteToken(CreateJwtToken(options, "refresh", userId, logger));

        if (accessToken is null || refreshToken is null)
        {
            logger.LogError("Ошибка при создании пары токенов.");
            throw new Domain.Exceptions.AuthenticationException();
        }

        logger.LogInformation("Пара токенов успешно создана для пользователя с ID: {UserId}", id);
        return new AuthDto(accessToken, refreshToken, userId);
    }

    private static JwtSecurityToken CreateJwtToken(IOptions<JwtOptions> options, string type, string userId,
        ILogger logger)
    {
        logger.LogInformation("Создание JWT токена типа: {Type} для пользователя с ID: {UserId}", type, userId);
        return new JwtSecurityToken(issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: GetClaims(userId, logger).Claims,
            expires: DateTime.UtcNow.AddDays(type == "access"
                ? options.Value.AccessTokenLifeTimeFromDays
                : options.Value.RefreshTokenLifeTimeFromDays),
            signingCredentials: new SigningCredentials(options.Value.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
    }

    private static ClaimsIdentity GetClaims(string userId, ILogger logger)
    {
        logger.LogInformation("Получение Claims для пользователя с ID: {UserId}", userId);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userId)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        logger.LogInformation("Claims успешно получены для пользователя с ID: {UserId}", userId);
        return claimsIdentity;
    }
}