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
        
        return (TokenHelper.GetJwtToken(username, user.Id, AccessTokenLifetime).ToString(),
            TokenHelper.GetJwtToken(username, user.Id, RefreshTokenLifetime).ToString());
    }

    public async Task<(string, string)> LogIn(string username, string password, CancellationToken cancellationToken)
    {
        var isUserExists = await usersRepository.IsUserExists(username, cancellationToken);
        if (!isUserExists)
        {
            throw new AlreadyExistsException("User does not exist");
        }
        
        var user = await usersRepository.GetUserByUsername(username, cancellationToken);
        if (!PasswordHashHelper.ValidateHash(password, user.PasswordSalt, user.Password))
        {
            throw new AuthenticationException("Wrong password");
        }
        
        return (TokenHelper.GetJwtToken(username, user.Id, AccessTokenLifetime).ToString(),
            TokenHelper.GetJwtToken(username, user.Id, RefreshTokenLifetime).ToString());
    }

    public async Task<(string, string)> RefreshTokens(string refreshToken, CancellationToken cancellationToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);
        var username = token.Claims.FirstOrDefault(x => x.Type == "Name")?.Value;
        var userId = token.Claims.FirstOrDefault(x => x.Type == "Name")?.Value;
        var isValidId = Guid.TryParse(userId, out var id);
        if (userId == null || username == null || !isValidId)
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        return (TokenHelper.GetJwtToken(username, id, AccessTokenLifetime).ToString(),
            TokenHelper.GetJwtToken(username, id, RefreshTokenLifetime).ToString());
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