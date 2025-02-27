using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Repositories;

public interface ICategoriesRepository
{
    Task AddCategory(Category category, CancellationToken cancellationToken);
    Task<bool> IsCategoryAlreadyExists(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken);
    Task<List<Category>> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken);

    Task<List<Category>> GetAllCategoriesByTransactionType(Guid userId, TransactionTypes transactionType, CancellationToken cancellationToken);
    Task<Category> GetCategoryById(Guid id, CancellationToken cancellationToken);
    Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    Task UpdateCategory(Category updatedCategory, CancellationToken cancellationToken);
    Task DeleteCategory(Category category, CancellationToken cancellationToken);
}