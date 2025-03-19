using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Notifications;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/notifications")]
public class NotificationsController(INotificationsService notificationsService) : ControllerBase
{
    [HttpPost("AddNotification")]
    public async Task<IActionResult> AddNotification([FromBody] AddNotificationRequest request,
        CancellationToken cancellationToken)
    {
        await notificationsService.AddNotification(request.UserId, request.Name, request.Description, request.DateTime,
            cancellationToken);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await notificationsService.GetNotifications(userId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken)
    {
        await notificationsService.UpdateNotification(request.NotificationId, request.Name, request.Description,
            request.DateTime, cancellationToken);
        return Ok();
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(Guid notificationId, CancellationToken cancellationToken)
    {
        await notificationsService.DeleteNotification(notificationId, cancellationToken);
        return Ok();
    }
}