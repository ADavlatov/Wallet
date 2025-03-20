using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class NotificationsRepositoryTests
{
    private WalletContext CreateInMemoryDbContext(List<Notification>? initialNotifications = null)
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new WalletContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (initialNotifications != null && initialNotifications.Any())
        {
            context.Notifications.AddRange(initialNotifications);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task AddNotification_ShouldAddNotificationToDatabase()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new NotificationsRepository(context);
        var userId = Guid.NewGuid();
        var notification = new Notification("Test Name", "Test Description", DateTime.UtcNow) { UserId = userId };

        // Act
        var result = await repository.AddNotification(notification, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var addedNotification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == result.Id);
        Assert.NotNull(addedNotification);
        Assert.Equal("Test Name", addedNotification.Name);
        Assert.Equal("Test Description", addedNotification.Description);
        Assert.Equal(notification.DateTime.ToUniversalTime().Ticks, addedNotification.DateTime.ToUniversalTime().Ticks,
            TimeSpan.TicksPerMillisecond);
        Assert.Equal(userId, addedNotification.UserId);
    }

    [Fact]
    public async Task GetNotificationById_ShouldReturnNotification_WhenNotificationExists()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        var notification = new Notification("Existing Name", "Existing Description", dateTime)
            { Id = notificationId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Notification> { notification });
        var repository = new NotificationsRepository(context);

        // Act
        var result = await repository.GetNotificationById(notificationId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notificationId, result.Id);
        Assert.Equal("Existing Name", result.Name);
        Assert.Equal("Existing Description", result.Description);
        Assert.Equal(dateTime.ToUniversalTime().Ticks, result.DateTime.ToUniversalTime().Ticks,
            TimeSpan.TicksPerMillisecond);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetNotificationById_ShouldThrowNotFoundException_WhenNotificationDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new NotificationsRepository(context);
        var notificationId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            repository.GetNotificationById(notificationId, CancellationToken.None));
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnNotificationsForGivenUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var dateTime1 = DateTime.UtcNow;
        var dateTime2 = DateTime.UtcNow.AddDays(1);
        var dateTime3 = DateTime.UtcNow.AddDays(2);
        var notifications = new List<Notification>
        {
            new("Name 1", "Description 1", dateTime1) { UserId = userId1 },
            new("Name 2", "Description 2", dateTime2) { UserId = userId1 },
            new("Name 3", "Description 3", dateTime3) { UserId = userId2 }
        };
        var context = CreateInMemoryDbContext(notifications);
        var repository = new NotificationsRepository(context);

        // Act
        var result = await repository.GetNotifications(userId1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, n => Assert.Equal(userId1, n.UserId));
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnEmptyList_WhenNoNotificationsForUser()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new NotificationsRepository(context);
        var userId = Guid.NewGuid();

        // Act
        var result = await repository.GetNotifications(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateNotification_ShouldUpdateExistingNotification()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        var existingNotification = new Notification("Old Name", "Old Description", dateTime)
            { Id = notificationId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Notification> { existingNotification });
        var repository = new NotificationsRepository(context);
        var updatedDateTime = DateTime.UtcNow.AddDays(1);

        // Act
        var notificationToUpdate = await context.Notifications.FindAsync(notificationId);
        Assert.NotNull(notificationToUpdate);
        notificationToUpdate.Name = "Updated Name";
        notificationToUpdate.Description = "Updated Description";
        notificationToUpdate.DateTime = updatedDateTime;

        await repository.UpdateNotification(notificationToUpdate, CancellationToken.None);

        // Assert
        var notificationFromDb = await context.Notifications.FindAsync(notificationId);
        Assert.NotNull(notificationFromDb);
        Assert.Equal("Updated Name", notificationFromDb.Name);
        Assert.Equal("Updated Description", notificationFromDb.Description);
        Assert.Equal(updatedDateTime.ToUniversalTime().Ticks, notificationFromDb.DateTime.ToUniversalTime().Ticks,
            TimeSpan.TicksPerMillisecond);
    }

    [Fact]
    public async Task DeleteNotification_ShouldRemoveExistingNotification()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;
        var notificationToDelete = new Notification("To Delete", "Description", dateTime)
            { Id = notificationId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Notification> { notificationToDelete });
        var repository = new NotificationsRepository(context);

        // Act
        await repository.DeleteNotification(notificationToDelete, CancellationToken.None);

        // Assert
        var notificationFromDb = await context.Notifications.FindAsync(notificationId);
        Assert.Null(notificationFromDb);
    }
}