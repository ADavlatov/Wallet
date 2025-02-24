using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IUsersService
{
    public Task AddUser(string username, string password, CancellationToken cancellationToken);
    public Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);
    public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken);
    public Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken);
    public Task DeleteUser(Guid id, CancellationToken cancellationToken);
}