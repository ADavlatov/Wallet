using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class CategoriesRepository(WalletContext db, ILogger<CategoriesRepository> logger) : ICategoriesRepository
{
    public async Task AddCategory(Category category, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на добавление категории. UserId: {category.UserId}, Name: {category.Name}, Type: {category.Type}");
        var isExists = await db.Categories
            .AnyAsync(x => x.UserId == category.UserId && x.Name == category.Name && x.Type == category.Type,
                cancellationToken);

        if (isExists)
        {
            throw new AlreadyExistsException("Category with this name and type already exists");
        }

        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Category>> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение всех категорий пользователя. UserId: {userId}");
        var categories = await db.Categories
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<List<Category>> GetAllCategoriesByTransactionType(Guid userId, TransactionTypes transactionType,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение категорий по типу транзакции. UserId: {userId}, TransactionType: {transactionType}");
        var categories = await db.Categories
            .Where(x => x.UserId == userId && x.Type == transactionType)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<Category> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение категории по ID. Id: {id}");
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }

        return category;
    }

    public async Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на получение категории по имени. UserId: {userId}, Name: {name}");
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }

        return category;
    }

    public async Task UpdateCategory(Category updatedCategory, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на обновление категории. Id: {updatedCategory.Id}, Name: {updatedCategory.Name}, Type: {updatedCategory.Type}");
        db.Categories.Update(updatedCategory);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategory(Category category, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Запрос на удаление категории. Id: {category.Id}");
        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
    }
}