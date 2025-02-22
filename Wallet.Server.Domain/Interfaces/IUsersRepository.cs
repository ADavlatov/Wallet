using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IUsersRepository
{
    Task<Result> AddUser(User user, CancellationToken cancellationToken);
    Task<Result<List<User>>> GetAllUsers(CancellationToken cancellationToken);
    Task<Result<User>> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<Result<User>> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task<Result> UpdateUser(User user, CancellationToken cancellationToken);
    Task<Result> DeleteUser(User user, CancellationToken cancellationToken);
}