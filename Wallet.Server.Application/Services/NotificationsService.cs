using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class NotificationsService(INotificationsRepository notificationsRepository, IUsersRepository usersRepository)
    : INotificationsService
{
    public async Task AddNotification(Guid userId, string name, string description, DateTime dateTime,
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        await notificationsRepository.AddNotification(
            new Notification(name, description, dateTime)
            {
                UserId = userId, 
                User = user
            }, 
            cancellationToken);
    }

    public async Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        return await notificationsRepository.GetNotifications(userId, cancellationToken);
    }

    public async Task UpdateNotification(Guid notificationId, string? name, string? description, DateTime? dateTime,
        CancellationToken cancellationToken)
    {
        var notification = await notificationsRepository.GetNotificationById(notificationId, cancellationToken);

        notification.Name = name ?? notification.Name;
        notification.Description = description ?? notification.Description;
        notification.DateTime = dateTime ?? notification.DateTime;
        
        await notificationsRepository.UpdateNotification(notification, cancellationToken);
    }

    public async Task DeleteNotification(Guid notificationId, CancellationToken cancellationToken)
    {
        var notification = await notificationsRepository.GetNotificationById(notificationId, cancellationToken);
        await notificationsRepository.DeleteNotification(notification, cancellationToken);
    }
}