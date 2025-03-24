using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class CategoriesService(
    ICategoriesRepository categoriesRepository,
    IUsersRepository usersRepository,
    ILogger<CategoriesService> logger)
    : ICategoriesService
{
    public async Task AddCategory(Guid userId, string name, TransactionTypes type, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на добавление категории. UserId: {UserId}, Name: {Name}, Type: {Type}.", userId,
            name, type);
        var user = await usersRepository.GetUserById(userId, cancellationToken);

        await categoriesRepository.AddCategory(
            new Category(name, type)
            {
                User = user,
                UserId = user.Id
            },
            cancellationToken);
    }

    public async Task<List<Category>> GetCategoriesByUser(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение категорий. UserId: {UserId}", userId);
        return await categoriesRepository.GetAllCategoriesByUserId(userId, cancellationToken);
    }

    public async Task<List<Category>> GetCategoriesByType(Guid userId, TransactionTypes type,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение категорий. UserId: {UserId}, Type: {Type}", userId, type);
        return await categoriesRepository.GetAllCategoriesByTransactionType(userId, type, cancellationToken);
    }

    public async Task<Category> GetCategoryByName(Guid userId, string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение категории. UserId: {UserId}, Name: {Name}", userId, name);
        return await categoriesRepository.GetCategoryByName(userId, name, cancellationToken);
    }

    public async Task<Category> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение категории. Id: {CategoryId}", categoryId);
        return await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
    }

    public async Task UpdateCategory(Guid categoryId, string? name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на обновление категории. Id: {CategoryId}, Name: {Name}", categoryId, name);
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);

        category.Name = name ?? category.Name;

        await categoriesRepository.UpdateCategory(category, cancellationToken);
    }

    public async Task DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на удаление категории. Id: {CategoryId}", categoryId);
        var category = await categoriesRepository.GetCategoryById(categoryId, cancellationToken);
        await categoriesRepository.DeleteCategory(category, cancellationToken);
    }
}