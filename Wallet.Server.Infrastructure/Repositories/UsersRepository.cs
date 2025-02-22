using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class UsersRepository(WalletContext db) : IUsersRepository
{
    public async Task<Result> AddUser(User user, CancellationToken cancellationToken)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<List<User>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await db.Users.ToListAsync(cancellationToken);
        if (!users.Any())
        {
            return Result.Fail("Users not found");
        }
        
        return Result.Ok(users);
    }

    public async Task<Result<User>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return Result.Fail("User not found");
        }
        
        return Result.Ok(user);
    }

    public async Task<Result<User>> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        if (user is null)
        {
            return Result.Fail("User not found");
        }
        
        return Result.Ok(user);
    }

    public async Task<Result> UpdateUser(User updatedUser, CancellationToken cancellationToken)
    {
        db.Users.Update(updatedUser);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteUser(User user, CancellationToken cancellationToken)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}