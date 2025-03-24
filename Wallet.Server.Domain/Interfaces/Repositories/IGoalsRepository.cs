using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface IGoalsRepository
{
    Task AddGoal(Goal goal, CancellationToken cancellationToken);
    
    Task AddSumToGoal(Goal goal, decimal sum, CancellationToken cancellationToken);
    
    Task<List<Goal>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken);
    
    Task<Goal> GetGoalById(Guid id, CancellationToken cancellationToken);
    
    Task UpdateGoal(Goal updatedGoal, CancellationToken cancellationToken);
    
    Task DeleteGoal(Goal goal, CancellationToken cancellationToken);
}