using FluentResults;
using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class CategoriesRepository(WalletContext db) : ICategoriesRepository
{
    public async Task<Result> AddCategory(Category category, CancellationToken cancellationToken)
    {
        await db.Categories.AddAsync(category, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<List<Category>>> GetAllCategoriesByTransactionType(Guid userId, TransactionTypes transactionType, CancellationToken cancellationToken)
    {
        var categories = await db.Categories.Where(x => x.UserId == userId && x.Type == transactionType).ToListAsync(cancellationToken);
        if (!categories.Any())
        {
            return Result.Fail("No categories found");
        }

        return Result.Ok(categories.ToList());
    }

    public async Task<Result<Category>> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (category is null)
        {
            return Result.Fail("Category not found");
        }

        return Result.Ok(category);
    }

    public async Task<Result<Category>> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        var category = await db.Categories.FirstOrDefaultAsync(x => x.UserId == userId && x.Name == name, cancellationToken);
        if (category is null)
        {
            return Result.Fail("Category not found");
        }

        return Result.Ok(category);
    }

    public async Task<Result> UpdateCategory(Category updatedCategory, CancellationToken cancellationToken)
    {
        db.Categories.Update(updatedCategory);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteCategory(Category category, CancellationToken cancellationToken)
    {
        db.Categories.Remove(category);
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}