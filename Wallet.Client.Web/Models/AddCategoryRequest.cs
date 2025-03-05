using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class AddCategoryRequest(string userId, string name, int type)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
    [JsonPropertyName("type")] public int Type { get; set; } = type;
}