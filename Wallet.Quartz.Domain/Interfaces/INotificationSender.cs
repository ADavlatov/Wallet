namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationSender
{
    Task SendNotification(string message);
}