using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Domain.Interfaces;

public interface ICategoriesRepository
{
    Task<IResult> AddCategory(Category category, CancellationToken cancellationToken);
    Task<IResult> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken);
    Task<IResult> GetCategoryById(Guid id, CancellationToken cancellationToken);
    Task<IResult> GetCategoryByName(string name, CancellationToken cancellationToken);
    Task<IResult> UpdateCategory(Category category, CancellationToken cancellationToken);
    Task<IResult> DeleteCategory(Category category, CancellationToken cancellationToken);
}