using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface ICategoriesService
{
    public Task AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken);
    public Task<List<Category>> GetCategoriesByUser(Guid userId, CancellationToken cancellationToken);
    public Task<List<Category>> GetCategoriesByType(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    public Task<Category> GetCategoryById(Guid categoryId, CancellationToken cancellationToken);
    public Task UpdateCategory(Guid categoryId, string? name, CancellationToken cancellationToken);
    public Task DeleteCategory(Guid categoryId, CancellationToken cancellationToken);
}