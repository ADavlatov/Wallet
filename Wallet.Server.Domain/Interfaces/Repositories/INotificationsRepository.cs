using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface INotificationsRepository
{
    public Task<Notification> AddNotification(Notification notification, CancellationToken cancellationToken);
    public Task<Notification> GetNotificationById(Guid notificationId, CancellationToken cancellationToken);
    public Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken);
    public Task UpdateNotification(Notification notification, CancellationToken cancellationToken);
    public Task DeleteNotification(Notification notification, CancellationToken cancellationToken);
}