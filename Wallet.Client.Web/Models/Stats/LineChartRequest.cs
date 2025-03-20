using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Stats;

public class LineChartRequest(string userId, string period)
{
    [JsonPropertyName("userId")] public string UserId { get; } = userId;
    [JsonPropertyName("period")] public string Period { get; } = period;
}