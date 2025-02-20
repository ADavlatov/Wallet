using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<IResult> AddTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<IResult> GetAllTransactionsByUserId(Guid userId, CancellationToken cancellationToken);
    Task<IResult> GetTransactionById(Guid id, CancellationToken cancellationToken);
    Task<IResult> GetTransactionByName(string name, CancellationToken cancellationToken);
    Task<IResult> UpdateTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<IResult> DeleteTransaction(Transaction transaction, CancellationToken cancellationToken);
}