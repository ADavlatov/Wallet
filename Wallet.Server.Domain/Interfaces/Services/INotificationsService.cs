using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface INotificationsService
{
    public Task AddNotification(Guid userId, string name, string description, DateTime dateTime, CancellationToken cancellationToken);
    public Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken);
    public Task UpdateNotification(Guid notificationId, string? name, string? description, DateTime? dateTime, CancellationToken cancellationToken);
    public Task DeleteNotification(Guid notificationId, CancellationToken cancellationToken);
}