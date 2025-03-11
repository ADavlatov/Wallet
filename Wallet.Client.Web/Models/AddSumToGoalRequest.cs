using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class AddSumToGoalRequest(string goalId, decimal sum)
{
    [JsonPropertyName("goalId")] public string GoalId { get; set; } = goalId;
    [JsonPropertyName("sum")] public decimal Sum { get; set; } = sum;
}