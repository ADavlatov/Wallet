using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class GoalsService(
    IGoalsRepository goalsRepository,
    IUsersRepository usersRepository,
    ILogger<GoalsService> logger) : IGoalsService
{
    public async Task AddGoal(Guid userId, string name, decimal amount, DateOnly? deadline,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на добавление цели. " +
                              "UserId: {UserId}, " +
                              "Name: {Name}, " +
                              "Amount: {Amount}, " +
                              "Deadline: {Deadline}", userId, name, amount, deadline);

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
        logger.LogInformation("Запрос на добавление суммы к цели. GoalId: {GoalId}, Sum: {Sum}", goalId, sum);
        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);
        await goalsRepository.AddSumToGoal(goal, sum, cancellationToken);
    }

    public async Task<List<Goal>> GetGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение целей пользователя. UserId: {UserId}", userId);
        return await goalsRepository.GetAllGoalsByUserId(userId, cancellationToken);
    }

    public async Task<Goal> GetGoalById(Guid goalId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение цели. GoalId: {GoalId}", goalId);
        return await goalsRepository.GetGoalById(goalId, cancellationToken);
    }

    public async Task UpdateGoal(Guid goalId, string? name, decimal? amount, DateOnly? deadline,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на обновление цели. " +
                              "GoalId: {GoalId}, " +
                              "Name: {Name}, " +
                              "Amount: {Amount}, " +
                              "Deadline: {Deadline}", goalId, name, amount, deadline);

        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);

        goal.Name = name ?? goal.Name;
        goal.TargetSum = amount ?? goal.TargetSum;
        goal.Deadline = deadline ?? goal.Deadline;

        await goalsRepository.UpdateGoal(goal, cancellationToken);
    }

    public async Task DeleteGoal(Guid goalId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на удаление цели. GoalId: {GoalId}", goalId);
        var goal = await goalsRepository.GetGoalById(goalId, cancellationToken);
        await goalsRepository.DeleteGoal(goal, cancellationToken);
    }
}