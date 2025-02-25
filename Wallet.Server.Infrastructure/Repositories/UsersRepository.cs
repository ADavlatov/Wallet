using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class UsersRepository(WalletContext db) : IUsersRepository
{
    public async Task<User> AddUser(User user, CancellationToken cancellationToken)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<bool> IsUserExists(string username, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        if (user is null)
        {
            return false;
        }
        
        return true;
    }

    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await db.Users.ToListAsync(cancellationToken);
        if (!users.Any())
        {
            throw new NotFoundException("Users not found");
        }
        
        return users;
    }

    public async Task<User> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }
        
        return user;
    }

    public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }
        
        return user;
    }

    public async Task UpdateUser(User updatedUser, CancellationToken cancellationToken)
    {
        db.Users.Update(updatedUser);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUser(User user, CancellationToken cancellationToken)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
    }
}