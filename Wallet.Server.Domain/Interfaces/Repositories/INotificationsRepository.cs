using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface INotificationsRepository
{
    Task<Notification> AddNotification(Notification notification, CancellationToken cancellationToken);

    Task<Notification> GetNotificationById(Guid notificationId, CancellationToken cancellationToken);

    Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken);

    Task UpdateNotification(Notification notification, CancellationToken cancellationToken);

    Task DeleteNotification(Notification notification, CancellationToken cancellationToken);
}