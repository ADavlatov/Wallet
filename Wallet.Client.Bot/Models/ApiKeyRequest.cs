using System.Text.Json.Serialization;

namespace Wallet.Client.Bot.Models;

public class ApiKeyRequest(string apiKey, long telegramUserId)
{
    [JsonPropertyName("apiKey")] public string ApiKey { get; set; } = apiKey;
    [JsonPropertyName("telegramUserId")] public long TelegramUserId { get; set; } = telegramUserId;
}