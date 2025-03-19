using Microsoft.AspNetCore.Mvc;
using Wallet.Quartz.Application.Models;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Presentation.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController(INotificationsService notificationsService) : ControllerBase
{
    [HttpPost("ScheduleNotification")]
    public async Task<IActionResult> ScheduleNotification([FromBody] ScheduleNotificationRequest request,
        CancellationToken cancellationToken)
    {
        await notificationsService.ScheduleNotification(request.Id, request.TelegramUserId, request.Name, request.Description,
            request.NotificationDateTime, cancellationToken);
        return Ok();
    }
}