using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wallet.Server.Application.Models.Notifications;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;
using Wallet.Server.Infrastructure.Options;

namespace Wallet.Server.Application.Services;

public class NotificationsService(
    INotificationsRepository notificationsRepository,
    IUsersRepository usersRepository,
    HttpClient httpClient,
    IOptions<UrlOptions> urlOptions,
    ILogger<NotificationsService> logger)
    : INotificationsService
{
    public async Task AddNotification(Guid userId, string name, string description, DateTime dateTime,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на добавление уведомления. UserId: {userId}, Name: {name}, Description: {description}, DateTime: {dateTime}");
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


        httpClient.BaseAddress = new Uri(urlOptions.Value.QuartzUrl);
        var request = new AddNotificationToQuartzRequest(notification.Id, (long)user.TelegramUserId,
            name, description, dateTime);
        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/notifications/ScheduleNotification",
            request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ArgumentException();
        }
    }

    public async Task<List<Notification>> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение уведомлений. UserId: {userId}");
        return await notificationsRepository.GetNotifications(userId, cancellationToken);
    }

    public async Task UpdateNotification(Guid notificationId, string? name, string? description, DateTime? dateTime,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление уведомления. NotificationId: {notificationId}, Name: {name}, Description: {description}, DateTime: {dateTime}");
        var notification = await notificationsRepository.GetNotificationById(notificationId, cancellationToken);

        notification.Name = name ?? notification.Name;
        notification.Description = description ?? notification.Description;
        notification.DateTime = dateTime ?? notification.DateTime;

        await notificationsRepository.UpdateNotification(notification, cancellationToken);
    }

    public async Task DeleteNotification(Guid notificationId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление уведомления. NotificationId: {notificationId}");
        var notification = await notificationsRepository.GetNotificationById(notificationId, cancellationToken);
        await notificationsRepository.DeleteNotification(notification, cancellationToken);
    }
}