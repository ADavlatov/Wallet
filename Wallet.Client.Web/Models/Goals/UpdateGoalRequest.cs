using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Goals;

public class UpdateGoalRequest(string goalId, string name, decimal targetSum, DateOnly? deadline)
{
    [JsonPropertyName("goalId")] public string GoalId { get; set; } = goalId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
    [JsonPropertyName("targetSum")] public decimal targetSum { get; set; } = targetSum;
    [JsonPropertyName("deadline")] public DateOnly? deadline { get; set; } = deadline;
}