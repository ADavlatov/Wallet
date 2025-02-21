using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Infrastructure.Repositories;

public class GoalsRepository : IGoalsRepository
{
    public Task<IResult> AddGoal(Goal goal, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAllGoalsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetGoalById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetGoalByName(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateGoal(Goal goal, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeleteGoal(Goal goal, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}