using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class CategoriesRepositoryIntegrationTests : IDisposable
{
    private readonly WalletContext _context;
    private readonly CategoriesRepository _repository;

    public CategoriesRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WalletContext(options);
        _context.Database.EnsureCreated();
        _repository = new CategoriesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddCategory_ShouldAddCategoryToDatabase()
    {
        // Arrange
        var category = new Category("Test Category", TransactionTypes.Expense) { UserId = Guid.NewGuid() };
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.AddCategory(category, cancellationToken);

        // Assert
        var addedCategory = await _context.Categories.FindAsync(category.Id);
        Assert.NotNull(addedCategory);
        Assert.Equal(category.Name, addedCategory.Name);
        Assert.Equal(category.Type, addedCategory.Type);
        Assert.Equal(category.UserId, addedCategory.UserId);
    }

    [Fact]
    public async Task IsCategoryAlreadyExists_ShouldReturnTrue_WhenCategoryExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string categoryName = "Existing Category";
        const TransactionTypes transactionType = TransactionTypes.Income;
        var cancellationToken = CancellationToken.None;
        var existingCategory = new Category(categoryName, transactionType) { UserId = userId };
        _context.Categories.Add(existingCategory);
        await _context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await _repository.IsCategoryAlreadyExists(userId, categoryName, transactionType, cancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsCategoryAlreadyExists_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string categoryName = "NonExisting Category";
        const TransactionTypes transactionType = TransactionTypes.Expense;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.IsCategoryAlreadyExists(userId, categoryName, transactionType, cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllCategoriesByUserId_ShouldReturnCorrectCategoriesForUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _context.Categories.AddRange(
            new Category("Category 1", TransactionTypes.Income) { UserId = userId1 },
            new Category("Category 2", TransactionTypes.Expense) { UserId = userId1 },
            new Category("Category 3", TransactionTypes.Income) { UserId = userId2 }
        );
        await _context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await _repository.GetAllCategoriesByUserId(userId1, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, category => Assert.Equal(userId1, category.UserId));
    }

    [Fact]
    public async Task GetAllCategoriesByTransactionType_ShouldReturnCorrectCategoriesForTransactionType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const TransactionTypes transactionType = TransactionTypes.Expense;
        var cancellationToken = CancellationToken.None;
        _context.Categories.AddRange(
            new Category("Category 1", TransactionTypes.Income) { UserId = userId },
            new Category("Category 2", TransactionTypes.Expense) { UserId = userId },
            new Category("Category 3", TransactionTypes.Expense) { UserId = Guid.NewGuid() }
        );
        await _context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await _repository.GetAllCategoriesByTransactionType(userId, transactionType, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, category => Assert.Equal(transactionType, category.Type));
        Assert.All(result, category => Assert.Equal(userId, category.UserId));
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var expectedCategory = new Category("Test Category", TransactionTypes.Income) { Id = categoryId, UserId = Guid.NewGuid() };
        _context.Categories.Add(expectedCategory);
        await _context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await _repository.GetCategoryById(categoryId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
    }

    [Fact]
    public async Task GetCategoryById_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _repository.GetCategoryById(categoryId, cancellationToken));
    }

    [Fact]
    public async Task GetCategoryByName_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string categoryName = "Test Category";
        var cancellationToken = CancellationToken.None;
        var expectedCategory = new Category(categoryName, TransactionTypes.Expense) { UserId = userId };
        _context.Categories.Add(expectedCategory);
        await _context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await _repository.GetCategoryByName(userId, categoryName, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(categoryName, result.Name);
    }

    [Fact]
    public async Task GetCategoryByName_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string categoryName = "NonExisting Category";
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _repository.GetCategoryByName(userId, categoryName, cancellationToken));
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategoryInDatabase()
    {
        // Arrange
        var category = new Category("Old Category", TransactionTypes.Income) { UserId = Guid.NewGuid() };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        var updatedCategory = new Category("Updated Category", TransactionTypes.Expense) { Id = category.Id, UserId = category.UserId };
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.UpdateCategory(updatedCategory, cancellationToken);

        // Assert
        var retrievedCategory = await _context.Categories.FindAsync(category.Id);
        Assert.NotNull(retrievedCategory);
        Assert.Equal(updatedCategory.Name, retrievedCategory.Name);
        Assert.Equal(updatedCategory.Type, retrievedCategory.Type);
    }

    [Fact]
    public async Task DeleteCategory_ShouldRemoveCategoryFromDatabase()
    {
        // Arrange
        var categoryToDelete = new Category("Category To Delete", TransactionTypes.Income) { UserId = Guid.NewGuid() };
        _context.Categories.Add(categoryToDelete);
        await _context.SaveChangesAsync();
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.DeleteCategory(categoryToDelete, cancellationToken);

        // Assert
        var retrievedCategory = await _context.Categories.FindAsync(categoryToDelete.Id);
        Assert.Null(retrievedCategory);
    }
}