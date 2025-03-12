using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class TransactionsRepository(WalletContext db) : ITransactionsRepository
{
    public async Task AddTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetAllTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        var transactions = await db.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && x.Type == type)
            .ToListAsync(cancellationToken);

        return transactions;
    }

    public async Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId,
        CancellationToken cancellationToken)
    {
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
        return await db.Transactions
            .Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByTypeAndPeriod(Guid userId, TransactionTypes type,
        DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        return await db.Transactions.Include(x => x.Category)
            .Where(t => t.Date >= startDate && t.Date <= endDate && t.Type == type)
            .ToListAsync(cancellationToken);
    }


    public async Task UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken)
    {
        db.Transactions.Update(updatedTransaction);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync(cancellationToken);
    }
}