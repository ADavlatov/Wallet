using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface ITransactionsService
{
    public Task AddTransaction(Guid userId, Guid categoryId, string? name, decimal amount,
        DateTime date, TransactionTypes type, CancellationToken cancellationToken);
    public Task<List<Transaction>> GetTransactionsByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    public Task<List<Transaction>> GetTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken);
    public Task<Transaction> GetTransactionById(Guid transactionId, CancellationToken cancellationToken);
    public Task UpdateTransaction(Guid transactionId, string? name, decimal? amount, DateTime? date,
        TransactionTypes? type, CancellationToken cancellationToken);
    public Task DeleteTransaction(Guid transactionId, CancellationToken cancellationToken);
}