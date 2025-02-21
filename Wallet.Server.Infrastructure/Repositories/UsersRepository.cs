using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    public Task<IResult> AddUser(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAllUsers(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateUser(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeleteUser(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}