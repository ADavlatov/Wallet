using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface IGoalsRepository
{
    Task<IResult> AddGoal(Goal goal, CancellationToken cancellationToken);
    Task<IResult> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken);
    Task<IResult> GetGoalById(Guid id, CancellationToken cancellationToken);
    Task<IResult> GetGoalByName(string name, CancellationToken cancellationToken);
    Task<IResult> UpdateGoal(Goal goal, CancellationToken cancellationToken);
    Task<IResult> DeleteGoal(Goal goal, CancellationToken cancellationToken);
}