using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class CategoriesRepositoryTests
{
    private readonly Mock<ILogger<CategoriesRepository>> _loggerMock = new();

    private WalletContext CreateInMemoryDbContext(List<Category>? initialCategories = null)
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new WalletContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (initialCategories != null && initialCategories.Any())
        {
            context.Categories.AddRange(initialCategories);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task AddCategory_ShouldAddCategoryToDatabase()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new CategoriesRepository(context, _loggerMock.Object);
        var category = new Category("Test Category", TransactionTypes.Expense) { UserId = Guid.NewGuid() };

        // Act
        await repository.AddCategory(category, CancellationToken.None);

        // Assert
        var addedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
        Assert.NotNull(addedCategory);
        Assert.Equal("Test Category", addedCategory.Name);
        Assert.Equal(TransactionTypes.Expense, addedCategory.Type);
    }

    [Fact]
    public async Task AddCategory_ShouldThrowAlreadyExistsException_WhenCategoryExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingCategory = new Category("Existing Category", TransactionTypes.Income) { UserId = userId };
        var context = CreateInMemoryDbContext(new List<Category> { existingCategory });
        var repository = new CategoriesRepository(context, _loggerMock.Object);
        var newCategory = new Category("Existing Category", TransactionTypes.Income) { UserId = userId };

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(() => repository.AddCategory(newCategory, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllCategoriesByUserId_ShouldReturnCategoriesForGivenUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var categories = new List<Category>
        {
            new("Category 1", TransactionTypes.Expense) { UserId = userId1 },
            new("Category 2", TransactionTypes.Income) { UserId = userId1 },
            new("Category 3", TransactionTypes.Expense) { UserId = userId2 }
        };
        var context = CreateInMemoryDbContext(categories);
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllCategoriesByUserId(userId1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(userId1, c.UserId));
    }

    [Fact]
    public async Task GetAllCategoriesByUserId_ShouldReturnEmptyList_WhenNoCategoriesForUser()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new CategoriesRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await repository.GetAllCategoriesByUserId(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllCategoriesByTransactionType_ShouldReturnCategoriesForGivenType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categories = new List<Category>
        {
            new("Category 1", TransactionTypes.Expense) { UserId = userId },
            new("Category 2", TransactionTypes.Income) { UserId = userId },
            new("Category 3", TransactionTypes.Expense) { UserId = userId }
        };
        var context = CreateInMemoryDbContext(categories);
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllCategoriesByTransactionType(userId, TransactionTypes.Expense, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(TransactionTypes.Expense, c.Type));
    }

    [Fact]
    public async Task GetAllCategoriesByTransactionType_ShouldReturnEmptyList_WhenNoCategoriesForType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var context = CreateInMemoryDbContext();
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllCategoriesByTransactionType(userId, TransactionTypes.Income, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", TransactionTypes.Expense) { Id = categoryId, UserId = Guid.NewGuid() };
        var context = CreateInMemoryDbContext(new List<Category> { category });
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetCategoryById(categoryId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Test Category", result.Name);
    }

    [Fact]
    public async Task GetCategoryById_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new CategoriesRepository(context, _loggerMock.Object);
        var categoryId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetCategoryById(categoryId, CancellationToken.None));
    }

    [Fact]
    public async Task GetCategoryByName_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category("Test Category", TransactionTypes.Expense) { UserId = userId };
        var context = CreateInMemoryDbContext(new List<Category> { category });
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetCategoryByName(userId, "Test Category", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Category", result.Name);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetCategoryByName_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new CategoriesRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var categoryName = "NonExisting Category";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetCategoryByName(userId, categoryName, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateExistingCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCategory = new Category("Old Name", TransactionTypes.Expense) { Id = categoryId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Category> { existingCategory });
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        var categoryToUpdate = await context.Categories.FindAsync(categoryId); 
        Assert.NotNull(categoryToUpdate);
        categoryToUpdate.Name = "New Name";
        categoryToUpdate.Type = TransactionTypes.Income;

        await repository.UpdateCategory(categoryToUpdate, CancellationToken.None);

        // Assert
        var categoryFromDb = await context.Categories.FindAsync(categoryId);
        Assert.NotNull(categoryFromDb);
        Assert.Equal("New Name", categoryFromDb.Name);
        Assert.Equal(TransactionTypes.Income, categoryFromDb.Type);
    }

    [Fact]
    public async Task DeleteCategory_ShouldRemoveExistingCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var categoryToDelete = new Category("To Delete", TransactionTypes.Expense) { Id = categoryId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Category> { categoryToDelete });
        var repository = new CategoriesRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteCategory(categoryToDelete, CancellationToken.None);

        // Assert
        var categoryFromDb = await context.Categories.FindAsync(categoryId);
        Assert.Null(categoryFromDb);
    }
}