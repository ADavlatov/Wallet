using Microsoft.Extensions.Logging;
using Quartz;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Infrastructure.Quartz;

public class NotificationJob(
    ILogger<NotificationJob> logger,
    INotificationsRepository notificationsRepository,
    INotificationSender notificationSender) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var notificationIdString = context.JobDetail.JobDataMap.GetString("notificationId");
        var title = context.JobDetail.JobDataMap.GetString("title");

        logger.LogInformation($"Выполнение Job для уведомления с ID: {notificationIdString}, Title: {title} в: {DateTime.Now:F}");

        //  Здесь нужно будет получить Notification из репозитория по ID и отправить уведомление
        //  В текущем примере просто отправляем уведомление с заголовком
        await notificationSender.SendNotification($"Уведомление: {title}"); // Используем EmailNotificationService
    }
}