using FluentResults;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface ICategoriesRepository
{
    Task<Result> AddCategory(Category category, CancellationToken cancellationToken);
    Task<Result<List<Category>>> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken);
    Task<Result<Category>> GetCategoryById(Guid id, CancellationToken cancellationToken);
    Task<Result<Category>> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken);
    Task<Result> UpdateCategory(Category category, CancellationToken cancellationToken);
    Task<Result> DeleteCategory(Category category, CancellationToken cancellationToken);
}