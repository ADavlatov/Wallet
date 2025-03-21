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
        logger.LogInformation(
            $"Запрос на добавление транзакции. UserId: {transaction.UserId}, CategoryId: {transaction.CategoryId}, Name: {transaction.Name}, Amount: {transaction.Amount}, Date: {transaction.Date}, Type: {transaction.Type}");
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetAllTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение транзакций по типу. UserId: {userId}, Type: {type}");
        var transactions = await db.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && x.Type == type)
            .ToListAsync(cancellationToken);

        return transactions;
    }

    public async Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение транзакций по категории. CategoryId: {categoryId}");
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
        logger.LogInformation($"Запрос на получение транзакции по ID. Id: {id}");
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
        logger.LogInformation($"Запрос на получение транзакции по имени. UserId: {userId}, Name: {name}");
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
        logger.LogInformation(
            $"Запрос на получение транзакций за период. UserId: {userId}, StartDate: {startDate}, EndDate: {endDate}");
        return await db.Transactions
            .Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByTypeAndPeriod(Guid userId, TransactionTypes type,
        DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Запрос на получение транзакций по типу и периоду. UserId: {userId}, Type: {type}, StartDate: {startDate}, EndDate: {endDate}");
        return await db.Transactions.Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate && t.Type == type)
            .ToListAsync(cancellationToken);
    }


    public async Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Запрос на обновление транзакции. Id: {updatedTransaction.Id}, CategoryId: {updatedTransaction.CategoryId}, Name: {updatedTransaction.Name}, Amount: {updatedTransaction.Amount}, Date: {updatedTransaction.Date}, Type: {updatedTransaction.Type}");
        db.Transactions.Update(updatedTransaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление транзакции. Id: {transaction.Id}");
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }
}