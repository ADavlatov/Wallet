using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IGoalsService
{
    Task AddGoal(Guid userId, string name, decimal amount, DateOnly? deadline, CancellationToken cancellationToken);

    Task AddSumToGoal(Guid goalId, decimal sum, CancellationToken cancellationToken);

    Task<List<Goal>> GetGoalsByUserId(Guid userId, CancellationToken cancellationToken);

    Task<Goal> GetGoalById(Guid goalId, CancellationToken cancellationToken);

    Task UpdateGoal(Guid goalId, string? name, decimal? amount, DateOnly? deadline, CancellationToken cancellationToken);

    Task DeleteGoal(Guid goalId, CancellationToken cancellationToken);
}