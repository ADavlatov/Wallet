using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationsService
{
    Task<Notification> ScheduleNotification(long telegramUserId, string name, string description, DateTime notificationTime);
}