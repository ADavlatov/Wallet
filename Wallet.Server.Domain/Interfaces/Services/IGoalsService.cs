using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IGoalsService
{
    public Task AddGoal(Guid userId, string name, decimal amount, DateOnly? deadline, CancellationToken cancellationToken);
    public Task<List<Goal>> GetGoalsByUserId(Guid userId, CancellationToken cancellationToken);
    public Task<Goal> GetGoalById(Guid goalId, CancellationToken cancellationToken);
    public Task UpdateGoal(Guid goalId, string? name, decimal? amount, DateOnly? deadline, CancellationToken cancellationToken);
    public Task DeleteGoal(Guid goalId, CancellationToken cancellationToken);
}