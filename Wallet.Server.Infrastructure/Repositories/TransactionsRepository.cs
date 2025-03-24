using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class TransactionsRepository(WalletContext db, ILogger<TransactionsRepository> logger) : ITransactionsRepository
{
    public async Task AddTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на добавление транзакции. " +
                              "UserId: {UserId}, " +
                              "CategoryId: {CategoryId}, " +
                              "Name: {Name}, " +
                              "Amount: {Amount}, " +
                              "Date: {Date}, " +
                              "Type: {Type}",
            transaction.UserId, transaction.CategoryId, transaction.Name,
            transaction.Amount, transaction.Date, transaction.Type);

        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetAllTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакций по типу. UserId: {UserId}, Type: {Type}", userId, type);
        var transactions = await db.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && x.Type == type)
            .ToListAsync(cancellationToken);

        return transactions;
    }

    public async Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакций по категории. CategoryId: {CategoryId}", categoryId);
        var transactions = await db.Transactions
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync(cancellationToken);

        if (!transactions.Any())
        {
            throw new NotFoundException("Transactions not found");
        }

        return transactions;
    }

    public async Task<Transaction> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакции по ID. Id: {Id}", id);
        var transaction = await db.Transactions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException("Transaction not found");
        }

        return transaction;
    }

    public async Task<Transaction> GetTransactionByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакции по имени. UserId: {UserId}, Name: {Name}", userId, name);
        var transaction = await db.Transactions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException("Transaction not found");
        }

        return transaction;
    }

    public async Task<List<Transaction>> GetTransactionsByPeriod(Guid userId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакций за период. " +
                              "UserId: {UserId}, " +
                              "StartDate: {StartDate}, " +
                              "EndDate: {EndDate}", userId, startDate, endDate);
        return await db.Transactions
            .Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByTypeAndPeriod(Guid userId, TransactionTypes type,
        DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение транзакций по типу и периоду. " +
                              "UserId: {UserId}, " +
                              "Type: {Type}, " +
                              "StartDate: {StartDate}, " +
                              "EndDate: {EndDate}", userId, type, startDate, endDate);
        return await db.Transactions
            .Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate && t.Type == type)
            .ToListAsync(cancellationToken);
    }


    public async Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на обновление транзакции. " +
                              "Id: {Id}, " +
                              "CategoryId: {CategoryId}," +
                              " Name: {Name}, " +
                              "Amount: {Amount}, " +
                              "Date: {Date}, " +
                              "Type: {Type}",
            updatedTransaction.Id, updatedTransaction.CategoryId, updatedTransaction.Name,
            updatedTransaction.Amount, updatedTransaction.Date, updatedTransaction.Type);

        db.Transactions.Update(updatedTransaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на удаление транзакции. Id: {Id}", transaction.Id);
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }
}