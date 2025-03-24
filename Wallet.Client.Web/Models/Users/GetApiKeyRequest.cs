using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Users;

public class GetApiKeyRequest(string userId)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
}