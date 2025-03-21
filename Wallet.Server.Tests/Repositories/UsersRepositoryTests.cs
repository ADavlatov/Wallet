using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Helpers;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class UsersRepositoryTests
{
    private readonly Mock<ILogger<UsersRepository>> _loggerMock = new();

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
        var repository = new UsersRepository(context, _loggerMock.Object);
        var username = "Test User";
        var password = "Test Password";
        var apiKey = "Test Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
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
        var username = "Existing User";
        var password = "Test Password";
        var apiKey = "Existing Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var existingUser = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { existingUser });
        var repository = new UsersRepository(context, _loggerMock.Object);
        var newUser = new User(username, [0x05, 0x06], [0x07, 0x08], "New Key");

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(() => repository.AddUser(newUser, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        var passwordHash1 = new byte[]{ 0x01 };
        var passwordSalt1 = new byte[]{ 0x02 };
        var user1 = new User("User 1", passwordHash1, passwordSalt1, "Key 1");
        var passwordHash2 = new byte[]{ 0x03 };
        var passwordSalt2 = new byte[]{ 0x04 };
        var user2 = new User("User 2", passwordHash2, passwordSalt2, "Key 2");
        var users = new List<User> { user1, user2 };
        var context = CreateInMemoryDbContext(users);
        var repository = new UsersRepository(context, _loggerMock.Object);

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
        var repository = new UsersRepository(context, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetAllUsers(CancellationToken.None));
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var username = "Test User";
        var password = "Test Password";
        var apiKey = "Test Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var user = new User(username, passwordHash, passwordSalt, apiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context, _loggerMock.Object);

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
        var repository = new UsersRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserById(userId, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var username = "Test User";
        var password = "Test Password";
        var apiKey = "Test Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var user = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context, _loggerMock.Object);

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
        var repository = new UsersRepository(context, _loggerMock.Object);
        var username = "Nonexising User";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserByUsername(username, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserByApiKey_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var username = "Test User";
        var password = "Test Password";
        var apiKey = "Test Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var user = new User(username, passwordHash, passwordSalt, apiKey);
        var context = CreateInMemoryDbContext(new List<User> { user });
        var repository = new UsersRepository(context, _loggerMock.Object);

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
        var repository = new UsersRepository(context, _loggerMock.Object);
        var apiKey = "Noneexisting Key";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetUserByApiKey(apiKey, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldUsername = "Old User";
        var oldPassword = "Old Password";
        var oldApiKey = "Old Key";
        var (oldPasswordHash, oldPasswordSalt) = PasswordHashHelper.HashPassword(oldPassword, _loggerMock.Object);
        var existingUser = new User(oldUsername, oldPasswordHash, oldPasswordSalt, oldApiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { existingUser });
        var repository = new UsersRepository(context, _loggerMock.Object);
        var newUsername = "New User";
        var newPassword = "New Password";
        var newApiKey = "New Key";
        var (newPasswordHash, newPasswordSalt) = PasswordHashHelper.HashPassword(newPassword, _loggerMock.Object);

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
        var username = "To Delete";
        var password = "Test Password";
        var apiKey = "Key";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var userToDelete = new User(username, passwordHash, passwordSalt, apiKey) { Id = userId };
        var context = CreateInMemoryDbContext(new List<User> { userToDelete });
        var repository = new UsersRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteUser(userToDelete, CancellationToken.None);

        // Assert
        var userFromDb = await context.Users.FindAsync(userId);
        Assert.Null(userFromDb);
    }
}