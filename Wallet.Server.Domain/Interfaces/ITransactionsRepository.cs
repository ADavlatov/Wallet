using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task<Result> AddTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<Result<List<Transaction>>> GetAllTransactionsByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Result<Transaction>> GetTransactionById(Guid id, CancellationToken cancellationToken);
    Task<Result<Transaction>> GetTransactionByName(string name, CancellationToken cancellationToken);
    Task<Result> UpdateTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<Result> DeleteTransaction(Transaction transaction, CancellationToken cancellationToken);
}