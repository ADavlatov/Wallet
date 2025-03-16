using Wallet.Quartz.Domain.Entities;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Quartz;

namespace Wallet.Quartz.Application.Services;

public class NotificationsService(
    INotificationsRepository notificationsRepository,
    QuartzNotificationScheduler notificationScheduler) : INotificationsService
{
    public async Task<Notification> ScheduleNotification(long telegramUserId, string name, string description, DateTime notificationDateTime)
    {
        var notification = new Notification(telegramUserId, name, description, notificationDateTime);

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