using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class GoalsRepository(WalletContext db, ILogger<GoalsRepository> logger) : IGoalsRepository
{
    public async Task AddGoal(Goal goal, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на добавление цели. " +
                              "UserId: {UserId}," +
                              " Name: {Name}," +
                              " Amount: {TargetSum}," +
                              " Deadline: {Deadline}", goal.UserId, goal.Name, goal.TargetSum, goal.Deadline);

        db.Goals.Add(goal);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSumToGoal(Goal goal, decimal sum, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на добавление суммы к цели. GoalId: {Id}, Sum: {Sum}", goal.Id, sum);
        goal.CurrentSum += sum;
        db.Goals.Update(goal);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Goal>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение всех целей пользователя. UserId: {UserId}", userId);
        var goals = await db.Goals
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return goals;
    }

    public async Task<Goal> GetGoalById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение цели по ID. Id: {Id}", id);
        var goal = await db.Goals
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (goal is null)
        {
            throw new NotFoundException("Goal not found");
        }

        return goal;
    }

    public async Task<Goal> GetGoalByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение цели по имени. UserId: {UserId}, Name: {Name}", userId, name);
        var goal = await db.Goals
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);

        if (goal is null)
        {
            throw new NotFoundException("Goal not found");
        }

        return goal;
    }

    public async Task UpdateGoal(Goal updatedGoal, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на обновление цели. " +
                              "Id: {Id}, Name: {Name}, Amount: {TargetSum}, Deadline: {Deadline}",
            updatedGoal.Id, updatedGoal.Name, updatedGoal.TargetSum, updatedGoal.Deadline);
        
        db.Goals.Update(updatedGoal);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteGoal(Goal goal, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на удаление цели. Id: {Id}", goal.Id);
        db.Goals.Remove(goal);
        await db.SaveChangesAsync(cancellationToken);
    }
}