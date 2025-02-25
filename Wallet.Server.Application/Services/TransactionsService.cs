using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Application.Services;

public class TransactionsService(
    ITransactionsRepository transactionsRepository,
    ICategoriesRepository categoriesRepository,
    IUsersRepository usersRepository) : ITransactionsService
{
    public async Task AddTransaction(Guid userId, Guid categoryId, string? name, decimal amount, DateTime date,
        TransactionTypes type, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
        await transactionsRepository.AddTransaction(new Transaction(user, category, name, amount, date, type), cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        return await transactionsRepository.GetAllTransactionsByType(userId, type, cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return await transactionsRepository.GetAllTransactionsByCategory(categoryId, cancellationToken);
    }


    public async Task<Transaction> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
    {
        return await transactionsRepository.GetTransactionById(transactionId, cancellationToken);
    }

    public async Task UpdateTransaction(Guid transactionId, string? name, decimal? amount, DateTime? date,
        TransactionTypes? type, CancellationToken cancellationToken)
    {
        var transaction = await transactionsRepository.GetTransactionById(transactionId, cancellationToken);
        
        transaction.Name = name ?? transaction.Name;
        transaction.Amount = amount ?? transaction.Amount;
        transaction.Date = date ?? transaction.Date;
        transaction.Type = type ?? transaction.Type;
        
        await transactionsRepository.UpdateTransaction(transaction, cancellationToken);
    }

    public async Task RemoveTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        var transaction = await transactionsRepository.GetTransactionById(transactionId, cancellationToken);
        await transactionsRepository.DeleteTransaction(transaction, cancellationToken);
    }
}