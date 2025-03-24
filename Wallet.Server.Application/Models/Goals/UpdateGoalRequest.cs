namespace Wallet.Server.Application.Models.Goals;

public record UpdateGoalRequest(
    Guid GoalId, 
    string? Name, 
    decimal? TargetSum, 
    DateOnly? Deadline);