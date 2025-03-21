using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Notifications;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/notifications")]
public class NotificationsController(INotificationsService notificationsService) : ControllerBase
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
        await notificationsService.AddNotification(request.UserId, request.Name, request.Description, request.DateTime,
            cancellationToken);
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
        return Ok(await notificationsService.GetNotifications(userId, cancellationToken));
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
        await notificationsService.UpdateNotification(request.NotificationId, request.Name, request.Description,
            request.DateTime, cancellationToken);
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
        await notificationsService.DeleteNotification(notificationId, cancellationToken);
        return Ok();
    }
}