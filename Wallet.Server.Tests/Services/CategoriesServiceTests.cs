using Moq;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;

namespace Wallet.Server.Tests.Services;

public class CategoriesServiceTests
{
    private readonly Mock<ICategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly CategoriesService _categoriesService;

    public CategoriesServiceTests()
    {
        _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _categoriesService = new CategoriesService(_categoriesRepositoryMock.Object, _usersRepositoryMock.Object);
    }

    [Fact]
    public async Task AddCategory_ShouldAddCategoryToRepository_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Food";
        var type = TransactionTypes.Expense;
        var user = new User("testuser", [], [], "testkey") { Id = userId };
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _categoriesService.AddCategory(userId, name, type, CancellationToken.None);

        // Assert
        _categoriesRepositoryMock.Verify(repo => repo.AddCategory(
            It.Is<Category>(c => c.Name == name && c.Type == type && c.UserId == userId && c.User == user),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCategory_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Food";
        var type = TransactionTypes.Expense;
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _categoriesService.AddCategory(userId, name, type, CancellationToken.None));

        _categoriesRepositoryMock.Verify(repo => repo.AddCategory(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCategoriesByUser_ShouldReturnCategoriesFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categories = new List<Category>
        {
            new Category("Food", TransactionTypes.Expense) { UserId = userId },
            new Category("Income", TransactionTypes.Income) { UserId = userId }
        };
        _categoriesRepositoryMock.Setup(repo => repo.GetAllCategoriesByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _categoriesService.GetCategoriesByUser(userId, CancellationToken.None);

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public async Task GetCategoriesByType_ShouldReturnCategoriesFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = TransactionTypes.Expense;
        var categories = new List<Category>
        {
            new("Food", TransactionTypes.Expense) { UserId = userId },
            new("Entertainment", TransactionTypes.Expense) { UserId = userId }
        };
        _categoriesRepositoryMock.Setup(repo =>
                repo.GetAllCategoriesByTransactionType(userId, type, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _categoriesService.GetCategoriesByType(userId, type, CancellationToken.None);

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public async Task GetCategoryByName_ShouldReturnCategoryFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Food";
        var category = new Category(name, TransactionTypes.Expense) { UserId = userId };
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByName(userId, name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _categoriesService.GetCategoryByName(userId, name, CancellationToken.None);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnCategoryFromRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Food", TransactionTypes.Expense) { Id = categoryId };
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _categoriesService.GetCategoryById(categoryId, CancellationToken.None);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategoryInRepository_WhenCategoryExistsAndNameIsProvided()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var newName = "Groceries";
        var existingCategory = new Category("Food", TransactionTypes.Expense) { Id = categoryId };
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        await _categoriesService.UpdateCategory(categoryId, newName, CancellationToken.None);

        // Assert
        Assert.Equal(newName, existingCategory.Name);
        _categoriesRepositoryMock.Verify(repo => repo.UpdateCategory(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateCategoryInRepository_WhenCategoryExistsAndNameIsNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category("Food", TransactionTypes.Expense) { Id = categoryId };
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        await _categoriesService.UpdateCategory(categoryId, null, CancellationToken.None);

        // Assert
        Assert.Equal("Food", existingCategory.Name); // Name should not change
        _categoriesRepositoryMock.Verify(repo => repo.UpdateCategory(existingCategory, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var newName = "Groceries";
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Category not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _categoriesService.UpdateCategory(categoryId, newName, CancellationToken.None));

        _categoriesRepositoryMock.Verify(
            repo => repo.UpdateCategory(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCategory_ShouldDeleteCategoryFromRepository_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryToDelete = new Category("Food", TransactionTypes.Expense) { Id = categoryId };
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryToDelete);

        // Act
        await _categoriesService.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        _categoriesRepositoryMock.Verify(repo => repo.DeleteCategory(categoryToDelete, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Category not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _categoriesService.DeleteCategory(categoryId, CancellationToken.None));

        _categoriesRepositoryMock.Verify(
            repo => repo.DeleteCategory(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}