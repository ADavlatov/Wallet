namespace Wallet.Quartz.Application.Models;

public record ScheduleNotificationRequest(
    Guid Id, 
    long TelegramUserId, 
    string Name, 
    string Description, 
    DateTime NotificationDateTime);