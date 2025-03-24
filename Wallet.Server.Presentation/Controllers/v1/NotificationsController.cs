using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Notifications;
using Wallet.Server.Application.Validators.Notifications;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

/// <summary>
/// Контроллер для работы с уведомлениями
/// </summary>
/// <param name="notificationsService">Сервис уведомлений</param>
[ApiController]
[Route("/api/v1/notifications")]
public class NotificationsController(
    INotificationsService notificationsService,
    ILogger<NotificationsController> logger) : ControllerBase
{
    /// <summary>
    /// Добавляет новое уведомление.
    /// </summary>
    /// <param name="request">Данные для добавления уведомления. Содержит UserId, Name, Description, DateTime</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [Authorize]
    [HttpPost("AddNotification")]
    public async Task<IActionResult> AddNotification([FromBody] AddNotificationRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало запроса на добавление уведомления. " +
                              "UserId: {UserId}, Name: {Name}.", request.UserId, request.Name);

        var validationResult = await new AddNotificationRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Ошибка валидации при добавлении уведомления. " +
                              "UserId: {request.UserId}, Name: {request.Name}.", request.UserId, request.Name);

            return BadRequest(validationResult.Errors);
        }

        await notificationsService
            .AddNotification(request.UserId, request.Name, request.Description, request.DateTime, cancellationToken);

        logger.LogInformation("Запрос на добавление уведомления завершен. " +
                              "UserId: {request.UserId}, Name: {request.Name}.", request.UserId, request.Name);

        return Ok();
    }

    /// <summary>
    /// Получает уведомления для конкретного пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список уведомлений для пользователя.</returns>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало запроса на получение уведомлений пользователя. UserId: {UserId}.", userId);
        var result = await notificationsService.GetNotifications(userId, cancellationToken);
        logger.LogInformation("Запрос на получение уведомлений пользователя завершен. " +
                              "UserId: {UserId}. Количество уведомлений: {Count}.", userId, result.Count);

        return Ok(result);
    }

    /// <summary>
    /// Обновляет существующее уведомление.
    /// </summary>
    /// <param name="request">Данные для обновления уведомления. Содержит NotificationId, Name nullable, Description nullable, DateTime nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало запроса на обновление уведомления. " +
                              "NotificationId: {NotificationId}, Name: {.Name}.", request.NotificationId, request.Name);

        var validationResult = await new UpdateNotificationRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Ошибка валидации при обновлении уведомления. " +
                              "NotificationId: {NotificationId}, Name: {Name}.", request.NotificationId, request.Name);

            return BadRequest(validationResult.Errors);
        }

        await notificationsService
            .UpdateNotification(request.NotificationId, request.Name, request.Description, request.DateTime,
                cancellationToken);

        logger.LogInformation("Запрос на обновление уведомления завершен. " +
                              "NotificationId: {NotificationId}.", request.NotificationId);

        return Ok();
    }

    /// <summary>
    /// Удаляет уведомление по его идентификатору.
    /// </summary>
    /// <param name="notificationId">Идентификатор уведомления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(Guid notificationId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало запроса на удаление уведомления. " +
                              "NotificationId: {NotificationId}.", notificationId);

        await notificationsService.DeleteNotification(notificationId, cancellationToken);

        logger.LogInformation("Запрос на удаление уведомления завершен. " +
                              "NotificationId: {NotificationId}.", notificationId);
        return Ok();
    }
}