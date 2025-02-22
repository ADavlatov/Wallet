using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IGoalsRepository
{
    Task<Result> AddGoal(Goal goal, CancellationToken cancellationToken);
    Task<Result<List<Goal>>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Result<Goal>> GetGoalById(Guid id, CancellationToken cancellationToken);
    Task<Result<Goal>> GetGoalByName(Guid userId, string name, CancellationToken cancellationToken);
    Task<Result> UpdateGoal(Goal updatedGoal, CancellationToken cancellationToken);
    Task<Result> DeleteGoal(Goal goal, CancellationToken cancellationToken);
}