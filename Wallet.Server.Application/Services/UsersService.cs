using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;
using Wallet.Server.Infrastructure.Helpers;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Application.Services;

public class UsersService(IUsersRepository usersRepository, IOptions<JwtOptions> options, ILogger<UsersService> logger) : IUsersService
{
    public async Task<AuthDto> SignUp(string username, string password, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на регистрацию пользователя. Username: {username}");
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, logger);
        var user = new User(username, passwordHash, passwordSalt, ApiKeyGenerator.GenerateApiKey(logger));
        var addedUser = await usersRepository.AddUser(user, cancellationToken);

        return TokenHelper.CreateTokensPair(options, addedUser.Id.ToString(), logger);
    }

    public async Task<AuthDto> SignIn(string username, string password, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на вход пользователя. Username: {username}");
        var user = await usersRepository.GetUserByUsername(username, cancellationToken);

        if (!PasswordHashHelper.ValidateHash(password, user.PasswordSalt, user.PasswordHash, logger))
        {
            throw new AuthenticationException("Wrong password");
        }

        return TokenHelper.CreateTokensPair(options, user.Id.ToString(), logger);
    }

    public async Task<AuthDto> RefreshTokens(string refreshToken, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление токенов.");
        var token = await new JwtSecurityTokenHandler().ValidateTokenAsync(refreshToken,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = options.Value.Issuer,
                ValidAudience = options.Value.Audience,
                IssuerSigningKey = options.Value.GetSymmetricSecurityKey()
            });

        if (!token.IsValid)
        {
            logger.LogWarning($"Ошибка при валидации refresh token: {refreshToken}");
            throw new AuthenticationException("Invalid refresh token");
        }

        var id = token.Claims.First().Value;
        if (!Guid.TryParse(id.ToString(), out var userId))
        {
            logger.LogWarning($"Ошибка при валидации refresh token: {refreshToken}");
            throw new AuthenticationException("Invalid refresh token");
        }

        return TokenHelper.CreateTokensPair(options, userId.ToString(), logger);
    }

    public async Task<User> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение пользователя. UserId: {userId}");
        return await usersRepository.GetUserById(userId, cancellationToken);
    }

    public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение пользователя. Username: {username}");
        return await usersRepository.GetUserByUsername(username, cancellationToken);
    }

    public async Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление пользователя. Id: {id}, Username: {username}");
        var user = await usersRepository.GetUserById(id, cancellationToken);

        user.Username = username ?? user.Username;
        user.Email = user.Email;
        if (password is not null)
        {
            var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, logger);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await usersRepository.UpdateUser(user, cancellationToken);
    }

    public async Task DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление пользователя. Id: {id}");
        var user = await usersRepository.GetUserById(id, cancellationToken);
        await usersRepository.DeleteUser(user, cancellationToken);
    }

    public async Task<string> GetApiKey(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение API ключа. UserId: {userId}");
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        return user.ApiKey;
    }

    public async Task UpdateApiKey(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление API ключа. UserId: {userId}");
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        user.ApiKey = ApiKeyGenerator.GenerateApiKey(logger);
        await usersRepository.UpdateUser(user, cancellationToken);
    }

    public async Task ValidateApiKey(string apiKey, long telegramUserId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на валидацию API ключа для Telegram. ApiKey: {apiKey}, TelegramUserId: {telegramUserId}");
        var user = await usersRepository.GetUserByApiKey(apiKey, cancellationToken);
        user.TelegramUserId = telegramUserId;
        await usersRepository.UpdateUser(user, cancellationToken);
    }
}