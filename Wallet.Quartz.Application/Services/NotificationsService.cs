using Wallet.Quartz.Domain.Entities;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Quartz;

namespace Wallet.Quartz.Application.Services;

public class NotificationsService(
    INotificationsRepository notificationsRepository,
    QuartzNotificationScheduler notificationScheduler) : INotificationsService
{
    public async Task<Notification> ScheduleNotification(string name, string description, DateTime notificationDateTime)
    {
        var notificationId = Guid.NewGuid();
        var notification = new Notification(name, description, notificationDateTime);

        await notificationsRepository.AddNotification(notification); //  Сохранение уведомления

        await notificationScheduler.ScheduleNotification(notification); //  Планирование основного уведомления
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