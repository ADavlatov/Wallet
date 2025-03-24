using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IUsersService
{
    Task<AuthDto> SignUp(string username, string password, CancellationToken cancellationToken);

    Task<AuthDto> SignIn(string username, string password, CancellationToken cancellationToken);

    Task<AuthDto> RefreshTokens(string refreshToken);

    Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);

    Task<User> GetUserByUsername(string username, CancellationToken cancellationToken);

    Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken);

    Task DeleteUser(Guid id, CancellationToken cancellationToken);

    Task<string> GetApiKey(Guid userId, CancellationToken cancellationToken);

    Task UpdateApiKey(Guid userId, CancellationToken cancellationToken);

    Task ValidateApiKey(string apiKey, long telegramUserId, CancellationToken cancellationToken);
}