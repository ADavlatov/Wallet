using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Helpers;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Tests.Services;

public class UsersServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IOptions<JwtOptions>> _jwtOptionsMock;
    private readonly Mock<ILogger<UsersService>> _loggerMock;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        _loggerMock = new Mock<ILogger<UsersService>>();
        var jwtOptions = new JwtOptions
        {
            Issuer = "testissuer",
            Audience = "testaudience",
            SecretKey = "this is a test secret key for jwt",
            AccessTokenLifeTimeFromDays = 1,
            RefreshTokenLifeTimeFromDays = 7
        };
        _jwtOptionsMock.Setup(o => o.Value).Returns(jwtOptions);
        _usersService = new UsersService(_usersRepositoryMock.Object, _jwtOptionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SignIn_ShouldReturnAuthDto_WhenCredentialsAreCorrect()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(password, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var user = new User(username, passwordHash, passwordSalt, "testapikey") { Id = userId };

        _usersRepositoryMock.Setup(repo => repo.GetUserByUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersService.SignIn(username, password, CancellationToken.None);

        // Assert
        _usersRepositoryMock.Verify(repo => repo.GetUserByUsername(username, CancellationToken.None), Times.Once);
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(userId.ToString(), result.UserId);
    }

    [Fact]
    public async Task SignIn_ShouldThrowAuthenticationException_WhenPasswordIsWrong()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var correctPassword = "testpassword";
        var (passwordHash, passwordSalt) = PasswordHashHelper.HashPassword(correctPassword, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var user = new User(username, passwordHash, passwordSalt, "testapikey") { Id = userId };

        _usersRepositoryMock.Setup(repo => repo.GetUserByUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _usersService.SignIn(username, password, CancellationToken.None));
        _usersRepositoryMock.Verify(repo => repo.GetUserByUsername(username, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RefreshTokens_ShouldReturnAuthDto_WhenRefreshTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var authDto = TokenHelper.CreateTokensPair(_jwtOptionsMock.Object, userId.ToString(), _loggerMock.Object);
        var refreshToken = authDto.RefreshToken;

        // Act
        var result = await _usersService.RefreshTokens(refreshToken, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(userId.ToString(), result.UserId);
    }

    [Fact]
    public async Task RefreshTokens_ShouldThrowAuthenticationException_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var invalidRefreshToken = "invalid.refresh.token";

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _usersService.RefreshTokens(invalidRefreshToken, CancellationToken.None));
    }

    [Fact]
    public async Task RefreshTokens_ShouldThrowAuthenticationException_WhenUserIdClaimIsInvalidGuid()
    {
        // Arrange
        var invalidId = "notaguid";
        var refreshToken = TokenHelper.CreateTokensPair(_jwtOptionsMock.Object, invalidId, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() => _usersService.RefreshTokens(refreshToken.RefreshToken, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUserFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), "apikey") { Id = userId };
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersService.GetUserById(userId, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetUserByUsername_ShouldReturnUserFromRepository()
    {
        // Arrange
        var username = "testuser";
        var user = new User(username, Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), "apikey");
        _usersRepositoryMock.Setup(repo => repo.GetUserByUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersService.GetUserByUsername(username, CancellationToken.None);

        // Assert
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUsernameAndCallRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User("olduser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), "apikey") { Id = userId };
        var newUsername = "newuser";

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        await _usersService.UpdateUser(userId, newUsername, null, CancellationToken.None);

        // Assert
        Assert.Equal(newUsername, existingUser.Username);
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(existingUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newUsername = "newuser";
        var newPassword = "newpassword";

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _usersService.UpdateUser(userId, newUsername, newPassword, CancellationToken.None));
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUserFromRepository_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userToDelete = new User("testuser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), "apikey") { Id = userId };
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userToDelete);

        // Act
        await _usersService.DeleteUser(userId, CancellationToken.None);

        // Assert
        _usersRepositoryMock.Verify(repo => repo.DeleteUser(userToDelete, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _usersService.DeleteUser(userId, CancellationToken.None));
        _usersRepositoryMock.Verify(repo => repo.DeleteUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetApiKey_ShouldReturnApiKeyFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var apiKey = "testapikey";
        var user = new User("testuser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), apiKey) { Id = userId };
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _usersService.GetApiKey(userId, CancellationToken.None);

        // Assert
        Assert.Equal(apiKey, result);
    }

    [Fact]
    public async Task GetApiKey_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _usersService.GetApiKey(userId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateApiKey_ShouldGenerateNewApiKeyAndUpdateUserInRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User("testuser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), "oldapikey") { Id = userId };

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        await _usersService.UpdateApiKey(userId, CancellationToken.None);

        // Assert
        Assert.NotEqual("oldapikey", existingUser.ApiKey);
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(existingUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateApiKey_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _usersService.UpdateApiKey(userId, CancellationToken.None));
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidateApiKey_ShouldUpdateTelegramUserIdAndCallRepository()
    {
        // Arrange
        var apiKey = "testapikey";
        var telegramUserId = 12345L;
        var existingUser = new User("testuser", Encoding.UTF8.GetBytes("hash"), Encoding.UTF8.GetBytes("salt"), apiKey) { Id = Guid.NewGuid() };

        _usersRepositoryMock.Setup(repo => repo.GetUserByApiKey(apiKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        await _usersService.ValidateApiKey(apiKey, telegramUserId, CancellationToken.None);

        // Assert
        Assert.Equal(telegramUserId, existingUser.TelegramUserId);
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(existingUser, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ValidateApiKey_ShouldThrowNotFoundException_WhenApiKeyDoesNotExist()
    {
        // Arrange
        var apiKey = "invalidapikey";
        var telegramUserId = 12345L;

        _usersRepositoryMock.Setup(repo => repo.GetUserByApiKey(apiKey, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _usersService.ValidateApiKey(apiKey, telegramUserId, CancellationToken.None));
        _usersRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}