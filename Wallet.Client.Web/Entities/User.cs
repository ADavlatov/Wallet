using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Entities;

public class User
{
    public string UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class UserModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("telegramUserId")] public long? TelegramUserId { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("passwordHash")] public string PasswordHash { get; set; }
    [JsonPropertyName("passwordSalt")] public string PasswordSalt { get; set; }
    [JsonPropertyName("transactions")] public List<Transaction> Transactions { get; set; }
    [JsonPropertyName("categories")] public List<Category> Categories { get; set; }
    [JsonPropertyName("goals")] public List<Goal> Goals { get; set; }
    [JsonPropertyName("notifications")] public List<Notification> Notifications { get; set; }
}