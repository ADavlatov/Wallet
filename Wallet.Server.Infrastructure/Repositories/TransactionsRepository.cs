using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class TransactionsRepository(WalletContext db) : ITransactionsRepository
{
    public async Task AddTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        await db.Transactions.AddAsync(transaction, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Transaction>> GetAllTransactionsByType(Guid categoryId, TransactionTypes type, CancellationToken cancellationToken)
    {
        var transactions = await db.Transactions.Where(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
        if (!transactions.Any())
        {
            throw new NotFoundException("Transactions not found");
        }
        
        return transactions;
    }

    public async Task<List<Transaction>> GetAllTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var transactions = await db.Transactions.Where(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
        if (!transactions.Any())
        {
            throw new NotFoundException("Transactions not found");
        }
        
        return transactions;
    }

    public async Task<Transaction> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transaction not found");
        }
        
        return transaction;
    }

    public async Task<Transaction> GetTransactionByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Transaction not found");
        }
        
        return transaction;
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