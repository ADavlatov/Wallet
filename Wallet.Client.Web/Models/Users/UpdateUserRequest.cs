using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Users;

public class UpdateUserRequest(string userId, string username, string password)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("username")] public string Username { get; set; } = username;
    [JsonPropertyName("password")] public string Password { get; set; } = password;
}