using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Tests.Services;

public class NotificationsServiceTests
{
    private readonly Mock<INotificationsRepository> _notificationsRepositoryMock;
    private readonly NotificationsService _notificationsService;

    public NotificationsServiceTests()
    {
        _notificationsRepositoryMock = new Mock<INotificationsRepository>();
        Mock<IUsersRepository> usersRepositoryMock = new();
        Mock<HttpClient> httpClientMock = new();
        Mock<IOptions<UrlOptions>> urlOptionsMock = new();
        Mock<ILogger<NotificationsService>> loggerMock = new();
        _notificationsService = new NotificationsService(
            _notificationsRepositoryMock.Object,
            usersRepositoryMock.Object,
            httpClientMock.Object,
            urlOptionsMock.Object, 
            loggerMock.Object);
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnNotificationsFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            new Notification("Notification 1", "Desc 1", DateTime.UtcNow) { UserId = userId },
            new Notification("Notification 2", "Desc 2", DateTime.UtcNow.AddDays(1)) { UserId = userId }
        };
        _notificationsRepositoryMock.Setup(repo => repo.GetNotifications(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        // Act
        var result = await _notificationsService.GetNotifications(userId, CancellationToken.None);

        // Assert
        Assert.Equal(notifications, result);
    }

    [Fact]
    public async Task
        UpdateNotification_ShouldUpdateNotificationInRepository_WhenNotificationExistsAndPropertiesAreProvided()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var existingNotification = new Notification("Old Name", "Old Desc", DateTime.UtcNow) { Id = notificationId };
        _notificationsRepositoryMock
            .Setup(repo => repo.GetNotificationById(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNotification);
        var newName = "New Name";
        var newDescription = "New Desc";
        var newDateTime = DateTime.UtcNow.AddDays(1);

        // Act
        await _notificationsService.UpdateNotification(notificationId, newName, newDescription, newDateTime,
            CancellationToken.None);

        // Assert
        Assert.Equal(newName, existingNotification.Name);
        Assert.Equal(newDescription, existingNotification.Description);
        Assert.Equal(newDateTime, existingNotification.DateTime);
        _notificationsRepositoryMock.Verify(
            repo => repo.UpdateNotification(existingNotification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task
        UpdateNotification_ShouldUpdateNotificationInRepository_WhenNotificationExistsAndSomePropertiesAreNull()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var existingNotification = new Notification("Old Name", "Old Desc", DateTime.UtcNow) { Id = notificationId };
        _notificationsRepositoryMock
            .Setup(repo => repo.GetNotificationById(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingNotification);
        string? newName = null;
        string? newDescription = null;
        DateTime? newDateTime = null;

        // Act
        await _notificationsService.UpdateNotification(notificationId, newName, newDescription, newDateTime,
            CancellationToken.None);

        // Assert
        Assert.Equal("Old Name", existingNotification.Name);
        Assert.Equal("Old Desc", existingNotification.Description);
        Assert.Equal(existingNotification.DateTime, existingNotification.DateTime);
        _notificationsRepositoryMock.Verify(
            repo => repo.UpdateNotification(existingNotification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateNotification_ShouldThrowNotFoundException_WhenNotificationDoesNotExist()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        _notificationsRepositoryMock
            .Setup(repo => repo.GetNotificationById(notificationId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Notification not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _notificationsService.UpdateNotification(notificationId, "New Name", "New Desc", DateTime.UtcNow.AddDays(1),
                CancellationToken.None));

        _notificationsRepositoryMock.Verify(
            repo => repo.UpdateNotification(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteNotification_ShouldDeleteNotificationFromRepository_WhenNotificationExists()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var notificationToDelete = new Notification("Test", "Desc", DateTime.UtcNow) { Id = notificationId };
        _notificationsRepositoryMock
            .Setup(repo => repo.GetNotificationById(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notificationToDelete);

        // Act
        await _notificationsService.DeleteNotification(notificationId, CancellationToken.None);

        // Assert
        _notificationsRepositoryMock.Verify(
            repo => repo.DeleteNotification(notificationToDelete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteNotification_ShouldThrowNotFoundException_WhenNotificationDoesNotExist()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        _notificationsRepositoryMock
            .Setup(repo => repo.GetNotificationById(notificationId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Notification not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _notificationsService.DeleteNotification(notificationId, CancellationToken.None));

        _notificationsRepositoryMock.Verify(
            repo => repo.DeleteNotification(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}