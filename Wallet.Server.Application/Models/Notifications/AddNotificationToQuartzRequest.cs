using System.Text.Json.Serialization;

namespace Wallet.Server.Application.Models.Notifications;

public class AddNotificationToQuartzRequest(
    Guid notificationId,
    long telegramUserId,
    string name,
    string description,
    DateTime dateTime)
{
    [JsonPropertyName("id")] public Guid NotificationId { get; set; } = notificationId;
    [JsonPropertyName("telegramUserId")] public long TelegramUserId { get; set; } = telegramUserId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
    [JsonPropertyName("description")] public string Description { get; set; } = description;
    [JsonPropertyName("notificationDateTime")] public DateTime NotificationDateTime { get; set; } = dateTime;
}