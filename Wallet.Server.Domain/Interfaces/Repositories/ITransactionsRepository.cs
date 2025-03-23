using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface ITransactionsRepository
{
    Task AddTransaction(Transaction transaction, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken);
    Task<Transaction> GetTransactionById(Guid id, CancellationToken cancellationToken);
    Task<List<Transaction>> GetTransactionsByPeriod(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task<List<Transaction>> GetTransactionsByTypeAndPeriod(Guid userId, TransactionTypes type, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken);
    Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken);
}