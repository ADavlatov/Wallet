using Wallet.Quartz.Domain.Entities;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Contexts;

namespace Wallet.Quartz.Infrastructure.Repositories;

public class NotificationsRepository(QuartzContext db) : INotificationsRepository
{
    public async Task AddNotification(Notification notification)
    {
        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
    }
}