using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces;

public interface ICategoriesService
{
    public Task AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken);
    public Task<List<Category>> GetCategories(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    public Task UpdateCategory(Guid categoryId, string? name, TransactionTypes? type, CancellationToken cancellationToken);
    public Task DeleteCategory(Guid categoryId, CancellationToken cancellationToken);
}