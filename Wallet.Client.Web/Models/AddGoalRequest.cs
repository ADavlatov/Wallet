using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class AddGoalRequest(string userId, string name, decimal targetSum, DateOnly? deadline)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
    [JsonPropertyName("targetSum")] public decimal TargetSum { get; set; } = targetSum;
    [JsonPropertyName("deadline")] public DateOnly? Deadline { get; set; } = deadline;
}