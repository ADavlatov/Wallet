namespace Wallet.Server.Application.Models.Goals;

public record AddGoalRequest(Guid UserId, string Name, decimal TargetSum, DateOnly? Deadline);