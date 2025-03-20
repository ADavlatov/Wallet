using Moq;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;

namespace Wallet.Server.Tests.Services;

public class TransactionsServiceTests
{
    private readonly Mock<ITransactionsRepository> _transactionsRepositoryMock;
    private readonly Mock<ICategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly TransactionsService _transactionsService;

    public TransactionsServiceTests()
    {
        _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
        _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _transactionsService = new TransactionsService(
            _transactionsRepositoryMock.Object,
            _categoriesRepositoryMock.Object,
            _usersRepositoryMock.Object);
    }

    [Fact]
    public async Task AddTransaction_ShouldAddTransactionToRepository_WhenUserAndCategoryExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Grocery Shopping";
        var amount = 50.75m;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var type = TransactionTypes.Expense;
        var user = new User("testuser", new byte[0], new byte[0], "testkey") { Id = userId };
        var category = new Category("Food", type) { Id = categoryId };

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        await _transactionsService.AddTransaction(userId, categoryId, name, amount, date, type, CancellationToken.None);

        // Assert
        _transactionsRepositoryMock.Verify(repo => repo.AddTransaction(
            It.Is<Transaction>(t =>
                t.Name == name &&
                t.Amount == amount &&
                t.Date == date &&
                t.Type == type &&
                t.UserId == userId &&
                t.User == user &&
                t.CategoryId == categoryId &&
                t.Category == category),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AddTransaction_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Grocery Shopping";
        var amount = 50.75m;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var type = TransactionTypes.Expense;

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _transactionsService.AddTransaction(userId, categoryId, name, amount, date, type, CancellationToken.None));

        _transactionsRepositoryMock.Verify(repo => repo.AddTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddTransaction_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Grocery Shopping";
        var amount = 50.75m;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var type = TransactionTypes.Expense;
        var user = new User("testuser", new byte[0], new byte[0], "testkey") { Id = userId };

        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Category not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _transactionsService.AddTransaction(userId, categoryId, name, amount, date, type, CancellationToken.None));

        _transactionsRepositoryMock.Verify(repo => repo.AddTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTransactionsByType_ShouldReturnTransactionsFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var type = TransactionTypes.Income;
        var transactions = new List<Transaction>
        {
            new Transaction(null, 100, DateOnly.FromDateTime(DateTime.Today), type) { UserId = userId },
            new Transaction(null, 200, DateOnly.FromDateTime(DateTime.Today), type) { UserId = userId },
            new Transaction(null, 50, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { UserId = userId }
        };
        _transactionsRepositoryMock.Setup(repo => repo.GetAllTransactionsByType(userId, type, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions.Where(t => t.Type == type).ToList());

        // Act
        var result = await _transactionsService.GetTransactionsByType(userId, type, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(type, t.Type));
    }

    [Fact]
    public async Task GetTransactionsByCategory_ShouldReturnTransactionsFromRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction(null, 50, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { CategoryId = categoryId },
            new Transaction(null, 75, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { CategoryId = categoryId },
            new Transaction(null, 100, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { CategoryId = Guid.NewGuid() }
        };
        _transactionsRepositoryMock.Setup(repo => repo.GetAllTransactionsByCategory(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions.Where(t => t.CategoryId == categoryId).ToList());

        // Act
        var result = await _transactionsService.GetTransactionsByCategory(categoryId, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(categoryId, t.CategoryId));
    }

    [Fact]
    public async Task GetTransactionById_ShouldReturnTransactionFromRepository()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction(null, 100, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Income) { Id = transactionId };
        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _transactionsService.GetTransactionById(transactionId, CancellationToken.None);

        // Assert
        Assert.Equal(transaction, result);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldUpdateTransactionInRepository_WhenTransactionAndCategoryExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = new Transaction("Old Name", 10.00m, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { Id = transactionId };
        var newCategoryId = Guid.NewGuid();
        var newCategory = new Category("New Category", TransactionTypes.Expense) { Id = newCategoryId };
        var newName = "New Name";
        var newAmount = 20.50m;
        var newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTransaction);
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(newCategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCategory);

        // Act
        await _transactionsService.UpdateTransaction(transactionId, newCategoryId, newName, newAmount, newDate, CancellationToken.None);

        // Assert
        Assert.Equal(newCategoryId, existingTransaction.CategoryId);
        Assert.Equal(newCategory, existingTransaction.Category);
        Assert.Equal(newName, existingTransaction.Name);
        Assert.Equal(newAmount, existingTransaction.Amount);
        Assert.Equal(newDate, existingTransaction.Date);
        _transactionsRepositoryMock.Verify(repo => repo.UpdateTransaction(existingTransaction, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldUpdateTransactionInRepository_WhenOnlyNullablePropertiesAreProvided()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = new Transaction("Old Name", 10.00m, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { Id = transactionId };
        string newName = "Updated Name";
        decimal? newAmount = 15.00m;
        DateOnly? newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTransaction);

        // Act
        await _transactionsService.UpdateTransaction(transactionId, null, newName, newAmount, newDate, CancellationToken.None);

        // Assert
        Assert.Equal("Updated Name", existingTransaction.Name);
        Assert.Equal(15.00m, existingTransaction.Amount);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today.AddDays(2)), existingTransaction.Date);
        _transactionsRepositoryMock.Verify(repo => repo.UpdateTransaction(existingTransaction, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldNotUpdateCategory_WhenCategoryIdIsNull()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = new Transaction(null, 0, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { Id = transactionId, CategoryId = Guid.NewGuid() };

        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTransaction);

        // Act
        await _transactionsService.UpdateTransaction(transactionId, null, "New Name", null, null, CancellationToken.None);

        // Assert
        _categoriesRepositoryMock.Verify(repo => repo.GetCategoryById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _transactionsRepositoryMock.Verify(repo => repo.UpdateTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var newName = "New Name";
        var newAmount = 20.50m;
        var newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Transaction not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _transactionsService.UpdateTransaction(transactionId, newCategoryId, newName, newAmount, newDate, CancellationToken.None));

        _categoriesRepositoryMock.Verify(repo => repo.GetCategoryById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _transactionsRepositoryMock.Verify(repo => repo.UpdateTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldThrowNotFoundException_WhenCategoryDoesNotExistDuringUpdate()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var existingTransaction = new Transaction(null, 0, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { Id = transactionId };
        var newCategoryId = Guid.NewGuid();

        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTransaction);
        _categoriesRepositoryMock.Setup(repo => repo.GetCategoryById(newCategoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Category not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _transactionsService.UpdateTransaction(transactionId, newCategoryId, null, null, null, CancellationToken.None));

        _transactionsRepositoryMock.Verify(repo => repo.UpdateTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTransaction_ShouldDeleteTransactionFromRepository_WhenTransactionExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transactionToDelete = new Transaction(null, 0, DateOnly.FromDateTime(DateTime.Today), TransactionTypes.Expense) { Id = transactionId };
        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionToDelete);

        // Act
        await _transactionsService.DeleteTransaction(transactionId, CancellationToken.None);

        // Assert
        _transactionsRepositoryMock.Verify(repo => repo.DeleteTransaction(transactionToDelete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTransaction_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        _transactionsRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Transaction not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _transactionsService.DeleteTransaction(transactionId, CancellationToken.None));

        _transactionsRepositoryMock.Verify(repo => repo.DeleteTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}