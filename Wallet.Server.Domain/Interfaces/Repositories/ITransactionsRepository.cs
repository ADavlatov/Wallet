using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces;

public interface ITransactionsRepository
{
    Task AddTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken);
    Task<Transaction> GetTransactionById(Guid id, CancellationToken cancellationToken);
    Task<Transaction> GetTransactionByName(Guid userId, string name, CancellationToken cancellationToken);
    Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken);
    Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken);
}