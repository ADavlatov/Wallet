using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class TransactionsService(
    ITransactionsRepository transactionsRepository,
    ICategoriesRepository categoriesRepository,
    IUsersRepository usersRepository,
    ILogger<TransactionsService> logger) : ITransactionsService
{
    public async Task AddTransaction(Guid userId, Guid categoryId, string? name, decimal amount, DateOnly date,
        TransactionTypes type, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на добавление транзакции. UserId: {userId}, CategoryId: {categoryId}, Name: {name}, Amount: {amount}, Date: {date}, Type: {type}");
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
        await transactionsRepository.AddTransaction(
            new Transaction(name, amount, date, type)
            {
                User = user,
                UserId = userId,
                Category = category,
                CategoryId = category.Id
            },
            cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение транзакций по типу. UserId: {userId}, Type: {type}");
        return await transactionsRepository.GetAllTransactionsByType(userId, type, cancellationToken);
    }

    public async Task<List<Transaction>> GetTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение транзакций по категории. CategoryId: {categoryId}");
        return await transactionsRepository.GetAllTransactionsByCategory(categoryId, cancellationToken);
    }


    public async Task<Transaction> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение транзакции. TransactionId: {transactionId}");
        return await transactionsRepository.GetTransactionById(transactionId, cancellationToken);
    }

    public async Task UpdateTransaction(Guid transactionId, Guid? categoryId, string? name, decimal? amount, DateOnly? date,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление транзакции. TransactionId: {transactionId}, CategoryId: {categoryId}, Name: {name}, Amount: {amount}, Date: {date}");
        var transaction = await transactionsRepository.GetTransactionById(transactionId, cancellationToken);

        if (categoryId is not null)
        {
            var category = await categoriesRepository.GetCategoryById(categoryId.Value, cancellationToken);
            transaction.CategoryId = category.Id;
            transaction.Category = category;
        }

        transaction.Name = name ?? transaction.Name;
        transaction.Amount = amount ?? transaction.Amount;
        transaction.Date = date ?? transaction.Date;

        await transactionsRepository.UpdateTransaction(transaction, cancellationToken);
    }

    public async Task DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление транзакции. TransactionId: {transactionId}");
        var transaction = await transactionsRepository.GetTransactionById(transactionId, cancellationToken);
        await transactionsRepository.DeleteTransaction(transaction, cancellationToken);
    }
}