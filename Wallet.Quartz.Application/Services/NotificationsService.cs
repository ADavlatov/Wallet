using Wallet.Quartz.Domain.Entities;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Quartz;

namespace Wallet.Quartz.Application.Services;

public class NotificationsService(
    INotificationsRepository notificationsRepository,
    INotificationsScheduler notificationScheduler) : INotificationsService
{
    public async Task<Notification> ScheduleNotification(Guid id, long telegramUserId, string name, string description,
        DateTime notificationDateTime, CancellationToken cancellationToken)
    {
        var notification = new Notification(id, telegramUserId, name, description, notificationDateTime);

        await notificationsRepository.AddNotification(notification);
        await notificationScheduler.ScheduleNotification(notification);

        if (notificationDateTime > DateTimeOffset.UtcNow.AddDays(1))
        {
            await notificationScheduler.ScheduleNotification(notification, TimeSpan.FromDays(1),
                $"{name} - за день");
        }

        if (notificationDateTime > DateTimeOffset.UtcNow.AddHours(1))
        {
            await notificationScheduler.ScheduleNotification(notification, TimeSpan.FromHours(1),
                $"{name} - за час");
        }

        return notification;
    }
}