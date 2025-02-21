using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Infrastructure.Repositories;

public class TransactionsRepository : ITransactionsRepository
{
    public Task<IResult> AddTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetAllTransactionsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> GetTransactionByName(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpdateTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeleteTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}