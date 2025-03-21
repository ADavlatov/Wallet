using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class GoalsRepository(WalletContext db) : IGoalsRepository
{
    public async Task AddGoal(Goal goal, CancellationToken cancellationToken)
    {
        db.Goals.Add(goal);
        await db.SaveChangesAsync(cancellationToken);
    }
    public async Task AddSumtoGoal(Goal goal, decimal sum, CancellationToken cancellationToken)
    {
        goal.CurrentSum += sum;
        db.Goals.Update(goal);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Goal>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var goals = await db.Goals
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return goals;
    }

    public async Task<Goal> GetGoalById(Guid id, CancellationToken cancellationToken)
    {
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
        db.Goals.Update(updatedGoal);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteGoal(Goal goal, CancellationToken cancellationToken)
    {
        db.Goals.Remove(goal);
        await db.SaveChangesAsync(cancellationToken);
    }
}