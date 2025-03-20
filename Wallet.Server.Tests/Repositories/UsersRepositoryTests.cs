using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Helpers;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class UsersRepositoryTests
{
    private WalletContext CreateInMemoryDbContext(List<User>? initialUsers = null)
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new WalletContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (initialUsers != null && initialUsers.Any())
        {
            context.Users.AddRange(initialUsers);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task AddUser_ShouldAddUserToDatabase()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new UsersRepository(context);
        var username = "testuser";
        var password = "testpassword";
        var apiKey = "testkey";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = new User(username, passwordHash, passwordSalt, apiKey);

        // Act
        var result = await repository.AddUser(user, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        Assert.NotNull(addedUser);
        Assert.Equal(username, addedUser.Username);
        Assert.Equal(passwordHash, addedUser.PasswordHash);
        Assert.Equal(passwordSalt, addedUser.PasswordSalt);
        Assert.Equal(apiKey, addedUser.ApiKey);
    }

    [Fact]
    public async Task AddUser_ShouldThrowAlreadyExistsException_WhenUserExists()
    {
        // Arrange
        var username = "existinguser";
        var password = "testpassword";
        var apiKey = "existingkey";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var existingUser = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { existingUser });
        var repository = new UsersRepository(context);
        var newUser = new User(username, [0x05, 0x06], [0x07, 0x08], "newkey");

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(() => repository.AddUser(newUser, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var passwordHash1 = new byte[]{ 0x01 };
        var passwordSalt1 = new byte[]{ 0x02 };
        var user1 = new User("user1", passwordHash1, passwordSalt1, "key1");
        var passwordHash2 = new byte[]{ 0x03 };
        var passwordSalt2 = new byte[]{ 0x04 };
        var user2 = new User("user2", passwordHash2, passwordSalt2, "key2");
        var users = new List<User> { user1, user2 };
        var context = CreateInMemoryDbContext(users);
        var repository = new UsersRepository(context);

        // Act
        var result = await repository.GetAllUsers(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllUsers_ShouldThrowNotFoundException_WhenNoUsersExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new UsersRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetAllUsers(CancellationToken.None));
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "testuser";
        var password = "testpassword";
        var apiKey = "testkey";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = new User(username, passwordHash, passwordSalt, apiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context);

        // Act
        var result = await repository.GetUserById(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(username, result.Username);
    }

    [Fact]
    public async Task GetUserById_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new UsersRepository(context);
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserById(userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var apiKey = "testkey";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context);

        // Act
        var result = await repository.GetUserByUsername(username, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new UsersRepository(context);
        var username = "nonexistinguser";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserByUsername(username, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserByApiKey_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var apiKey = "testkey";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var user = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context);

        // Act
        var result = await repository.GetUserByApiKey(apiKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(apiKey, result.ApiKey);
    }

    [Fact]
    public async Task GetUserByApiKey_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new UsersRepository(context);
        var apiKey = "nonexistingkey";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserByApiKey(apiKey, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldUsername = "olduser";
        var oldPassword = "oldpassword";
        var oldApiKey = "oldkey";
        var (oldPasswordHash, oldPasswordSalt) = PasswordHashHelper.HashPassword(oldPassword);
        var existingUser = new User(oldUsername, oldPasswordHash, oldPasswordSalt, oldApiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { existingUser });
        var repository = new UsersRepository(context);
        var newUsername = "newuser";
        var newPassword = "newpassword";
        var newApiKey = "newkey";
        var (newPasswordHash, newPasswordSalt) = PasswordHashHelper.HashPassword(newPassword);

        // Act
        var userToUpdate = await context.Users.FindAsync(userId);
        Assert.NotNull(userToUpdate);
        userToUpdate.Username = newUsername;
        userToUpdate.PasswordHash = newPasswordHash;
        userToUpdate.PasswordSalt = newPasswordSalt;
        userToUpdate.ApiKey = newApiKey;

        await repository.UpdateUser(userToUpdate, CancellationToken.None);

        // Assert
        var userFromDb = await context.Users.FindAsync(userId);
        Assert.NotNull(userFromDb);
        Assert.Equal(newUsername, userFromDb.Username);
        Assert.Equal(newPasswordHash, userFromDb.PasswordHash);
        Assert.Equal(newPasswordSalt, userFromDb.PasswordSalt);
        Assert.Equal(newApiKey, userFromDb.ApiKey);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveExistingUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "todelete";
        var password = "testpassword";
        var apiKey = "key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password);
        var userToDelete = new User(username, passwordHash, passwordSalt, apiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { userToDelete });
        var repository = new UsersRepository(context);

        // Act
        await repository.DeleteUser(userToDelete, CancellationToken.None);

        // Assert
        var userFromDb = await context.Users.FindAsync(userId);
        Assert.Null(userFromDb);
    }
}