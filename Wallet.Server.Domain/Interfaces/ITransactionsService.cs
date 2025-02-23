using FluentResults;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces;

public interface ITransactionsService
{
    public Task<Result> AddTransaction(Guid userId, Guid transactionId, string name, decimal amount,
        DateTime date, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result<List<Transaction>>> GetTransactions(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result<Transaction>> GetTransactionById(Guid transactionId, CancellationToken cancellationToken);
    public Task<Result> UpdateTransaction(Guid transactionId, string name, decimal amount, DateTime date,
        TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result> RemoveTransaction(Guid transactionId, CancellationToken cancellationToken);
}