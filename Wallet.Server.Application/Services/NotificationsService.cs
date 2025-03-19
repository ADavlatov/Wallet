using System.Net.Http.Headers;
using System.Net.Http.Json;
using Wallet.Server.Application.Models.Notifications;
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
        if (user.TelegramUserId == null)
        {
            throw new ArgumentException();
        }

        var notification = await notificationsRepository.AddNotification(
            new Notification(name, description, dateTime)
            {
                UserId = userId,
                User = user
            },
            cancellationToken);
        
       ;
       using var httpClient = new HttpClient();
        var request =
            new AddNotificationToQuartzRequest(notification.Id, (long)user.TelegramUserId, name, description, dateTime);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5230/api/v1/notifications/ScheduleNotification");
        httpRequest.Content = JsonContent.Create(request);

        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new ArgumentException();
        }
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