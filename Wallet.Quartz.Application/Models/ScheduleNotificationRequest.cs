namespace Wallet.Quartz.Application.Models;

public record ScheduleNotificationRequest(long TelegramUserId, string Name, string Description, DateTime NotificationDateTime);