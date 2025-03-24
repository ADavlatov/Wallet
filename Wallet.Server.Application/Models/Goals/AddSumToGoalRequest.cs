namespace Wallet.Server.Application.Models.Goals;

public record AddSumToGoalRequest(
    Guid GoalId, 
    decimal Sum);