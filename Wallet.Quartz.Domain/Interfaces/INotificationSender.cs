namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationSender
{
    Task SendNotification(long telegramUserId, string name, string description);
}