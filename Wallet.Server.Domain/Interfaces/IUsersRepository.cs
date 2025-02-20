using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IUsersRepository
{
    Task<IResult> AddUser(User user, CancellationToken cancellationToken);
    Task<IResult> GetAllUsers(CancellationToken cancellationToken);
    Task<IResult> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<IResult> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task<IResult> UpdateUser(User user, CancellationToken cancellationToken);
    Task<IResult> DeleteUser(User user, CancellationToken cancellationToken);
}