using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Stats;

public class GetExcelFileRequest(string userId, string period)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("period")] public string Period { get; set; } = period;
}