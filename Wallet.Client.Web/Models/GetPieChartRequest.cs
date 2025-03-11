using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class GetPieChartRequest(string userId, string period, int type)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("period")] public string Period { get; set; } = period;
    [JsonPropertyName("type")] public int Type { get; set; } = type;
}