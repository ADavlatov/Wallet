using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class CategoriesRepository(WalletContext db) : ICategoriesRepository
{
    public async Task AddCategory(Category category, CancellationToken cancellationToken)
    {
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsCategoryAlreadyExists(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name && x.Type == type, cancellationToken);
        return category is not null;
    }

    public async Task<List<Category>> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var categories = await db.Categories
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<List<Category>> GetAllCategoriesByTransactionType(Guid userId, TransactionTypes transactionType, CancellationToken cancellationToken)
    {
        var categories = await db.Categories
            .Where(x => x.UserId == userId && x.Type == transactionType)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<Category> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
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
        db.Categories.Update(updatedCategory);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategory(Category category, CancellationToken cancellationToken)
    {
        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
    }
}