namespace Wallet.Server.Application.Models.Notifications;

public record AddNotificationRequest(Guid UserId, string Name, string Description, DateTime DateTime);