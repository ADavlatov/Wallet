using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationsService
{
    Task<Notification> ScheduleNotification(string name, string description, DateTime notificationTime);
}