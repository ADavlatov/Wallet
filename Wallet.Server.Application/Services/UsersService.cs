using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Wallet.Server.Application.Models.Users;
using Wallet.Server.Domain;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;
using Wallet.Server.Infrastructure.Helpers;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Wallet.Server.Application.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    private const int AccessTokenLifetimeDays = 1;
    private const int RefreshTokenLifetimeDays = 15;

    public async Task<TokensDto> SignUp(string username, string password, CancellationToken cancellationToken)
    {
        var isUserExists = await usersRepository.IsUserExists(username, cancellationToken);
        if (isUserExists)
        {
            throw new AlreadyExistsException("User with this username already exists");
        }

        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = await usersRepository.AddUser(new User(username, passwordHash, passwordSalt), cancellationToken);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var accessToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(user.Id.ToString(), AccessTokenLifetimeDays));
        var refreshToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(user.Id.ToString(), RefreshTokenLifetimeDays));
        
        if (accessToken is null || refreshToken is null) throw new Domain.Exceptions.AuthenticationException();
        return new TokensDto(accessToken, refreshToken);
    }

    public async Task<TokensDto> SignIn(string username, string password, CancellationToken cancellationToken)
    {
        var isUserExists = await usersRepository.IsUserExists(username, cancellationToken);
        if (!isUserExists)
        {
            throw new NotFoundException("User does not exist");
        }

        var user = await usersRepository.GetUserByUsername(username, cancellationToken);
        if (!PasswordHashHelper.ValidateHash(password, user.PasswordSalt, user.PasswordHash))
        {
            throw new AuthenticationException("Wrong password");
        }

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var accessToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(user.Id.ToString(), AccessTokenLifetimeDays));
        var refreshToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(user.Id.ToString(), RefreshTokenLifetimeDays));
        
        if (accessToken is null || refreshToken is null) throw new Domain.Exceptions.AuthenticationException();
        return new TokensDto(accessToken, refreshToken);
    }

    public async Task<TokensDto> RefreshTokens(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await new JwtSecurityTokenHandler().ValidateTokenAsync(refreshToken, new TokenValidationParameters());
        var id = token.Claims.First().Value;
        if (!Guid.TryParse(id.ToString(), out var userId))
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var accessToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(userId.ToString(), AccessTokenLifetimeDays));
        var newRefreshToken = jwtSecurityTokenHandler.WriteToken(TokenHelper.CreateJwtToken(userId.ToString(), RefreshTokenLifetimeDays));
       
        if (accessToken is null || refreshToken is null) throw new Domain.Exceptions.AuthenticationException();
        return new TokensDto(accessToken, newRefreshToken);
    }

    public async Task<User> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return await usersRepository.GetUserById(userId, cancellationToken);
    }

    public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        return await usersRepository.GetUserByUsername(username, cancellationToken);
    }

    public async Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(id, cancellationToken);
        
        user.Username = username ?? user.Username;
        if (password is not null)
        {
            var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await usersRepository.UpdateUser(user, cancellationToken);
    }

    public async Task DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(id, cancellationToken);
        await usersRepository.DeleteUser(user, cancellationToken);
    }
}