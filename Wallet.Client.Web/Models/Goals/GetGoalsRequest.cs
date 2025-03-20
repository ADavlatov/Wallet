using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Goals;

public class GetGoalsRequest(string userId)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
}