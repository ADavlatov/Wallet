using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class NotificationsRepository(WalletContext db, ILogger<NotificationsRepository> logger) : INotificationsRepository
{
    public async Task<Notification> AddNotification(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на добавление уведомления. UserId: {notification.UserId}, Name: {notification.Name}, DateTime: {notification.DateTime}");
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task<Notification> GetNotificationById(Guid notificationId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение уведомления по ID. Id: {notificationId}");
        var notification = await db.Notifications
            .FirstOrDefaultAsync(x => x.Id == notificationId, cancellationToken);

        if (notification is null)
        {
            throw new NotFoundException();
        }

        return notification;
    }

    public async Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение уведомлений пользователя. UserId: {userId}");
        var notifications = await db.Notifications
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return notifications;
    }

    public async Task UpdateNotification(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление уведомления. Id: {notification.Id}, Name: {notification.Name}, Description: {notification.Description}, DateTime: {notification.DateTime}");
        db.Notifications.Update(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNotification(Notification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление уведомления. Id: {notification.Id}");
        db.Notifications.Remove(notification);
        await db.SaveChangesAsync(cancellationToken);
    }
}