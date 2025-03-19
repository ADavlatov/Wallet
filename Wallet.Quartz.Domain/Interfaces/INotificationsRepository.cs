using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationsRepository
{
    Task AddNotification(Notification notification);
}