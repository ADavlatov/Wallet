using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<User> AddUser(User user, CancellationToken cancellationToken);
    Task<bool> IsUserExists(string username, CancellationToken cancellationToken);
    Task<List<User>> GetAllUsers(CancellationToken cancellationToken);
    Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);
    Task<User> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task UpdateUser(User updatedUser, CancellationToken cancellationToken);
    Task DeleteUser(User user, CancellationToken cancellationToken);
}