using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class GoalsRepository(WalletContext db) : IGoalsRepository
{
    public async Task AddGoal(Goal goal, CancellationToken cancellationToken)
    {
        await db.Goals.AddAsync(goal, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Goal>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var goals = await db.Goals.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        if (!goals.Any())
        {
            throw new NotFoundException("Goals not found");
        }

        return goals;
    }

    public async Task<Goal> GetGoalById(Guid id, CancellationToken cancellationToken)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (goal is null)
        {
            throw new NotFoundException("Goal not found");
        }

        return goal;
    }

    public async Task<Goal> GetGoalByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
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