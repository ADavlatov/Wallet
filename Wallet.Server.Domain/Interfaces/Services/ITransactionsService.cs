using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface ITransactionsService
{
    Task AddTransaction(Guid userId, Guid categoryId, string? name, decimal amount, DateOnly date,
        TransactionTypes type, CancellationToken cancellationToken);

    Task<List<Transaction>> GetTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken);

    Task<List<Transaction>> GetTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken);

    Task<Transaction> GetTransactionById(Guid transactionId, CancellationToken cancellationToken);

    Task UpdateTransaction(Guid transactionId, Guid? categoryId, string? name, decimal? amount,
        DateOnly? date, CancellationToken cancellationToken);

    Task DeleteTransaction(Guid transactionId, CancellationToken cancellationToken);
}