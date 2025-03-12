using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationsScheduler
{
    Task ScheduleNotification(Notification notification);
    Task ScheduleNotification(Notification notification, TimeSpan delayBefore, string customName);
}