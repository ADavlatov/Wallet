using FluentResults;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces;

public interface ICategoriesService
{
    public Task<Result> AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result<List<Category>>> GetCategories(Guid userId, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result<Category>> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    public Task<Result> UpdateCategory(Guid categoryId, string name, TransactionTypes type, CancellationToken cancellationToken);
    public Task<Result> DeleteCategory(Guid categoryId, CancellationToken cancellationToken);
}