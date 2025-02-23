using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task AddTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Transaction> GetTransactionById(Guid id, CancellationToken cancellationToken);
    Task<Transaction> GetTransactionByName(Guid userId, string name, CancellationToken cancellationToken);
    Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken);
    Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken);
}