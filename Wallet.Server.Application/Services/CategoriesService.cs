using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Application.Services;

public class CategoriesService(ICategoriesRepository categoriesRepository, IUsersRepository usersRepository) : ICategoriesService
{
    public async Task AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserById(userId, cancellationToken);
        var category = await categoriesRepository.IsCategoryAlreadyExists(userId, name, type, cancellationToken);
        if (category)
        {
            throw new AlreadyExistsException("Category with this name and type already exists");
        }
        
        await categoriesRepository.AddCategory(new Category(user, name, type), cancellationToken);
    }

    public async Task<List<Category>> GetCategoriesByUser(Guid userId, CancellationToken cancellationToken)
    {
        return await categoriesRepository.GetAllCategoriesByUserId(userId, cancellationToken);
    }

    public async Task<List<Category>> GetCategoriesByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken)
    {
        return await categoriesRepository.GetAllCategoriesByTransactionType(userId, type, cancellationToken);
    }

    public async Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        return await categoriesRepository.GetCategoryByName(userId, name, cancellationToken);
    }

    public async Task UpdateCategory(Guid categoryId, string? name, TransactionTypes? type, CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
        
        category.Name = name ?? category.Name;
        category.Type = type ?? category.Type;
        
        await categoriesRepository.UpdateCategory(category, cancellationToken);
    }

    public async Task DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
        await categoriesRepository.DeleteCategory(category, cancellationToken);
    }
}