using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IUsersService
{
    public Task<AuthDto> SignUp(string username, string password, CancellationToken cancellationToken);
    public Task<AuthDto> SignIn(string username, string password, CancellationToken cancellationToken);
    public Task<AuthDto> RefreshTokens(string refreshToken, CancellationToken cancellationToken);
    public Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);
    public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken);
    public Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken);
    public Task DeleteUser(Guid id, CancellationToken cancellationToken);
    public Task<string> GetApiKey(Guid userId, CancellationToken cancellationToken);
    public Task UpdateApiKey(Guid userId, CancellationToken cancellationToken);
}