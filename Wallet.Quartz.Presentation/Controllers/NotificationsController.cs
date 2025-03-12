using Microsoft.AspNetCore.Mvc;
using Wallet.Quartz.Application.Models;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Presentation.Controllers;

[Route("api/v1/notifications")]
[ApiController]
public class NotificationsController(INotificationsService notificationsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ScheduleNotification([FromBody] ScheduleNotificationRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Title is required.");
        }
        if (request.NotificationDateTime == default)
        {
            return BadRequest("NotificationDateTime is required.");
        }

        var notification = await notificationsService.ScheduleNotification(request.Name, request.Description, request.NotificationDateTime);
        return Ok($"Уведомление '{notification.Name}' запланировано на {notification.NotificationDateTime:F}");
    }
}