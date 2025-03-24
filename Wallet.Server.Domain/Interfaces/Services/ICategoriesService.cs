using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface ICategoriesService
{
    Task AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken);
    
    Task<List<Category>> GetCategoriesByUser(Guid userId, CancellationToken cancellationToken);
    
    Task<List<Category>> GetCategoriesByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    
    Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    
    Task<Category> GetCategoryById(Guid categoryId, CancellationToken cancellationToken);
    
    Task UpdateCategory(Guid categoryId, string? name, CancellationToken cancellationToken);
    
    Task DeleteCategory(Guid categoryId, CancellationToken cancellationToken);
}