using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IUsersService
{
    public Task<Result> AddUser(string username, string password, CancellationToken cancellationToken);
    public Task<Result<User>> GetUserById(Guid userId, CancellationToken cancellationToken);
    public Task<Result<User>> GetUserByUsername(string username, CancellationToken cancellationToken);
    public Task<Result> UpdateUser(Guid id, string username, string password, CancellationToken cancellationToken);
    public Task<Result> DeleteUser(Guid id, CancellationToken cancellationToken);
}