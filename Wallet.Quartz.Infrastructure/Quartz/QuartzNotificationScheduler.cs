using Microsoft.Extensions.Logging;
using Quartz;
using Wallet.Quartz.Domain.Entities;
using Wallet.Quartz.Domain.Interfaces;

namespace Wallet.Quartz.Infrastructure.Quartz;

public class QuartzNotificationScheduler(
    ISchedulerFactory schedulerFactory,
    ILogger<QuartzNotificationScheduler> logger) : INotificationsScheduler
{
    public async Task ScheduleNotification(Notification notification)
    {
        await ScheduleNotification(notification, TimeSpan.Zero, notification.Name);
    }

    public async Task ScheduleNotification(Notification notification, TimeSpan delayBefore, string customName)
    {
        IScheduler scheduler = await schedulerFactory.GetScheduler();

        IJobDetail jobDetail = JobBuilder.Create<NotificationJob>()
            .WithIdentity($"notificationJob-{notification.Id}-{delayBefore.TotalMinutes}min")
            .UsingJobData("notificationId", notification.Id.ToString())
            .UsingJobData("name", notification.Name)
            .UsingJobData("description", notification.Description)
            .UsingJobData("userId", notification.TelegramUserId.ToString())
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"reminderTrigger-{notification.Id}-{delayBefore.TotalMinutes}min")
            .StartAt(notification.NotificationDateTime.Subtract(delayBefore))
            .Build();
        
        await scheduler.ScheduleJob(jobDetail, trigger);
        logger.LogInformation($"Job {jobDetail.Key} scheduled for {trigger.StartTimeUtc} with title '{customName}'");
    }
}