using Microsoft.AspNetCore.Http.HttpResults;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Infrastructure.Contexts;

namespace Wallet.Server.Infrastructure.Repositories;

public class CategoriesRepository(WalletContext db) : ICategoriesRepository
{
    public async Task<IResult> AddCategory(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetAllCategoriesByUserId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> GetCategoryByName(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> UpdateCategory(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> DeleteCategory(Category category, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}