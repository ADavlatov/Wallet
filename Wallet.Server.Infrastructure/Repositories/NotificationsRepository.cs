using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class NotificationsRepository(WalletContext db) : INotificationsRepository
{
    public async Task AddNotification(Notification notification, CancellationToken cancellationToken)
    {
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Notification> GetNotificationById(Guid notificationId, CancellationToken cancellationToken)
    {
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
        var notifications = await db.Notifications
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return notifications;
    }

    public async Task UpdateNotification(Notification notification, CancellationToken cancellationToken)
    {
        db.Notifications.Update(notification);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteNotification(Notification notification, CancellationToken cancellationToken)
    {
        db.Notifications.Remove(notification);
        await db.SaveChangesAsync(cancellationToken);
    }
}