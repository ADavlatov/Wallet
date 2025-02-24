using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Application.Services;

public class UsersService(IUsersRepository repository) : IUsersService
{
    public async Task AddUser(string username, string password, CancellationToken cancellationToken)
    {
        var user = await repository.IsUserExists(username, cancellationToken);
        if (user)
        {
            throw new AlreadyExistsException("User with this username already exists");
        }
        
        await repository.AddUser(new User(username, password), cancellationToken);
    }

    public async Task<User> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return await repository.GetUserById(userId, cancellationToken);
    }

    public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        return await repository.GetUserByUsername(username, cancellationToken);
    }

    public async Task UpdateUser(Guid id, string? username, string? password, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserById(id, cancellationToken);

        user.Username = username ?? user.Username;
        user.Password = password ?? user.Password;
        
        await repository.UpdateUser(user, cancellationToken);
    }

    public async Task DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserById(id, cancellationToken);
        await repository.DeleteUser(user, cancellationToken);
    }
}