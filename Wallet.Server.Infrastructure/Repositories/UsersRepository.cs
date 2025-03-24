using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class UsersRepository(WalletContext db, ILogger<UsersRepository> logger) : IUsersRepository
{
    public async Task<User> AddUser(User user, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на добавление пользователя. Username: {user.Username}");
        var isExists = await db.Users
            .AnyAsync(x => x.Username == user.Username, cancellationToken);

        if (isExists)
        {
            throw new AlreadyExistsException("User already exists");
        }

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение всех пользователей.");
        var users = await db.Users
            .ToListAsync(cancellationToken);

        if (!users.Any())
        {
            throw new NotFoundException("Users not found");
        }

        return users;
    }

    public async Task<User> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение пользователя по ID. UserId: {userId}");
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }

    public async Task<User> GetUserByUsername(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение пользователя по имени. Username: {username}");
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }

    public async Task<User> GetUserByApiKey(string apiKey, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение пользователя по API ключу. ApiKey: {apiKey}");
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.ApiKey == apiKey, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }

    public async Task UpdateUser(User updatedUser, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление пользователя. Id: {updatedUser.Id}, Username: {updatedUser.Username}");
        db.Users.Update(updatedUser);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUser(User user, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление пользователя. Id: {user.Id}");
        db.Users.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
    }
}