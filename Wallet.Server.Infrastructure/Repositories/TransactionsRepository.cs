using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class TransactionsRepository(WalletContext db) : ITransactionsRepository
{
    public async Task<Result> AddTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        await db.Transactions.AddAsync(transaction, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<List<Transaction>>> GetAllTransactionsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var transactions = await db.Transactions.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        if (!transactions.Any())
        {
            return Result.Fail("transactions not found");
        }
        
        return Result.Ok(transactions);
    }

    public async Task<Result<Transaction>> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (transaction is null)
        {
            return Result.Fail("transaction not found");
        }
        
        return Result.Ok(transaction);
    }

    public async Task<Result<Transaction>> GetTransactionByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
        if (transaction is null)
        {
            return Result.Fail("transaction not found");
        }
        
        return Result.Ok(transaction);
    }

    public async Task<Result> UpdateTransaction(Transaction updatedTransaction, CancellationToken cancellationToken)
    {
        db.Transactions.Update(updatedTransaction);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}