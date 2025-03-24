using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Notifications;

public class AddNotificationRequest(string userId, string name, string description, DateTime dateTime)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
    [JsonPropertyName("description")] public string Description { get; set; } = description;
    [JsonPropertyName("dateTime")] public DateTime DateTime { get; set; } = dateTime;
}