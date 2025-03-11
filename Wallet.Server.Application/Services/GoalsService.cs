using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class GoalsService(IGoalsRepository goalsRepository, IUsersRepository usersRepository) : IGoalsService
{

    public async Task AddGoal(Guid userId, string name, decimal amount, DateOnly? deadline, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        await goalsRepository.AddGoal(
            new Goal(name, amount, deadline)
            {
                UserId = userId,
                User = user
            },
            cancellationToken);
    }
    public async Task AddSumToGoal(Guid goalId, decimal sum, CancellationToken cancellationToken)
    {
        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);
        await goalsRepository.AddSumtoGoal(goal, sum, cancellationToken);
    }
    public async Task<List<Goal>> GetGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await goalsRepository.GetAllGoalsByUserId(userId, cancellationToken);
    }
    public async Task<Goal> GetGoalById(Guid goalId, CancellationToken cancellationToken)
    {
        return await goalsRepository.GetGoalById(goalId, cancellationToken);
    }
    public async Task UpdateGoal(Guid goalId, string? name, decimal? amount, DateOnly? deadline, CancellationToken cancellationToken)
    {
        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);
        
        goal.Name = name ?? goal.Name;
        goal.TargetSum = amount ?? goal.TargetSum;
        goal.Deadline = deadline ?? goal.Deadline;
        
        await goalsRepository.UpdateGoal(goal, cancellationToken);
    }
    public async Task DeleteGoal(Guid goalId, CancellationToken cancellationToken)
    {
        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);
        await goalsRepository.DeleteGoal(goal, cancellationToken);
    }
}