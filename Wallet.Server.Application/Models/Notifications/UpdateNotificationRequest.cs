namespace Wallet.Server.Application.Models.Notifications;

public record UpdateNotificationRequest(Guid NotificationId, string? Name, string? Description, DateTime? DateTime);