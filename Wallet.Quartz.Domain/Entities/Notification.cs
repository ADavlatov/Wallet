namespace Wallet.Quartz.Domain.Entities;

public class Notification(long telegramUserId, string name, string description, DateTime notificationDateTime)
{
    public Guid Id { get; set; }
    public long TelegramUserId { get; set; } = telegramUserId;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public DateTime NotificationDateTime { get; set; } = notificationDateTime;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}