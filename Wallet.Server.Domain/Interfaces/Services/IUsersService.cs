using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IUsersService
{
    public Task<TokensDto> SignUp(string username, string password, CancellationToken cancellationToken);
    public Task<TokensDto> SignIn(string username, string password, CancellationToken cancellationToken);
    public Task<TokensDto> RefreshTokens(string refreshToken, CancellationToken cancellationToken);
    public Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);
    public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken);
    public Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken);
    public Task DeleteUser(Guid id, CancellationToken cancellationToken);
}