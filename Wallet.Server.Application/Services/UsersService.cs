using System.IdentityModel.Tokens.Jwt;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Helpers;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Wallet.Server.Application.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    private const int AccessTokenLifetime = 1;
    private const int RefreshTokenLifetime = 15;

    public async Task<(string, string)> SignIn(string username, string password, CancellationToken cancellationToken)
    {
        var isUserExists = await usersRepository.IsUserExists(username, cancellationToken);
        if (isUserExists)
        {
            throw new AlreadyExistsException("User with this username already exists");
        }

        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = await usersRepository.AddUser(new User(username, passwordHash, passwordSalt), cancellationToken);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(user.Id.ToString(), AccessTokenLifetime));
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(user.Id.ToString(), RefreshTokenLifetime));
        
        return (accessToken, refreshToken);
    }

    public async Task<(string, string)> LogIn(string username, string password, CancellationToken cancellationToken)
    {
        var isUserExists = await usersRepository.IsUserExists(username, cancellationToken);
        if (!isUserExists)
        {
            throw new NotFoundException("User does not exist");
        }

        var user = await usersRepository.GetUserByUsername(username, cancellationToken);
        if (!PasswordHashHelper.ValidateHash(password, user.PasswordSalt, user.Password))
        {
            throw new AuthenticationException("Wrong password");
        }

        var accessToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(user.Id.ToString(), AccessTokenLifetime));
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(user.Id.ToString(), RefreshTokenLifetime));
        
        return (accessToken ?? throw new Domain.Exceptions.AuthenticationException(),
            refreshToken ?? throw new Domain.Exceptions.AuthenticationException());
    }

    public async Task<(string, string)> RefreshTokens(string refreshToken, CancellationToken cancellationToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
        var userId = token.Claims.First().Value;
        if (!Guid.TryParse(userId, out _))
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        var accessToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(userId, AccessTokenLifetime));
        var newRefreshToken = new JwtSecurityTokenHandler().WriteToken(TokenHelper.GetJwtToken(userId, RefreshTokenLifetime));
        
        return (accessToken ?? throw new Domain.Exceptions.AuthenticationException(),
            newRefreshToken ?? throw new Domain.Exceptions.AuthenticationException());
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
        user.Password = password ?? user.Password;

        await usersRepository.UpdateUser(user, cancellationToken);
    }

    public async Task DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(id, cancellationToken);
        await usersRepository.DeleteUser(user, cancellationToken);
    }
}