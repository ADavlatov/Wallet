using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Catergories;

public class GetCategoriesRequest(string userId, int type)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("type")] public int Type { get; set; } = type;
}