using Microsoft.Extensions.Logging;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Infrastructure.Notifications;

public class EmailNotificationSender(ILogger<EmailNotificationSender> logger) : INotificationSender
{
    public async Task SendNotification(string message)
    {
        logger.LogInformation($"Отправка уведомления через почту: {message}");
    }
}