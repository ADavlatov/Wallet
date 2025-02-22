using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class GoalsRepository(WalletContext db) : IGoalsRepository
{
    public async Task<Result> AddGoal(Goal goal, CancellationToken cancellationToken)
    {
        await db.Goals.AddAsync(goal, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<List<Goal>>> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var goals = await db.Goals.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        if (!goals.Any())
        {
            return Result.Fail("goals not found");
        }

        return Result.Ok();
    }

    public async Task<Result<Goal>> GetGoalById(Guid id, CancellationToken cancellationToken)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (goal is null)
        {
            return Result.Fail("goal not found");
        }
        
        return Result.Ok(goal);
    }

    public async Task<Result<Goal>> GetGoalByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        var goal = await db.Goals.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
        if (goal is null)
        {
            return Result.Fail("goal not found");
        }
        
        return Result.Ok(goal);
    }

    public async Task<Result> UpdateGoal(Goal updatedGoal, CancellationToken cancellationToken)
    {
        db.Goals.Update(updatedGoal);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteGoal(Goal goal, CancellationToken cancellationToken)
    {
        db.Goals.Remove(goal);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}