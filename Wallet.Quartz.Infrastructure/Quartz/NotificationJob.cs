using Microsoft.Extensions.Logging;
using Quartz;
using Wallet.Quartz.Domain.Interfaces;
using Wallet.Quartz.Infrastructure.Senders;

namespace Wallet.Quartz.Infrastructure.Quartz;

public class NotificationJob(
    ILogger<NotificationJob> logger,
    INotificationSender notificationSender,
    HttpClient httpClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var notificationIdString = context.JobDetail.JobDataMap.GetString("notificationId");
        var name = context.JobDetail.JobDataMap.GetString("name");
        var description = context.JobDetail.JobDataMap.GetString("description");
        var userId = context.JobDetail.JobDataMap.GetString("userId");

        if (name is null || description is null || userId is null)
        {
            throw new InvalidOperationException("Данные для уведомления не найдены");
        }

        logger.LogInformation(
            $"Выполнение Job для уведомления с ID: {notificationIdString}, " +
            $"Name: {name}, Description: {description} " +
            $"в: {DateTime.Now:F}");

        await DeleteNotificationFromDb(notificationIdString);
        await notificationSender.SendNotification(long.Parse(userId), name, description);
    }

    private async Task DeleteNotificationFromDb(string notificationIdString)
    {
        var response = await httpClient.DeleteAsync($"http://localhost:5221/api/v1/notifications/{notificationIdString}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("");
        }
    }
}