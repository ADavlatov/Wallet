namespace Wallet.Quartz.Application.Models;

public record ScheduleNotificationRequest(string Name, string Description, DateTime NotificationDateTime);