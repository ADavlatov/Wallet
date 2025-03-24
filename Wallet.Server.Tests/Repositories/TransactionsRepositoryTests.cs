using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class TransactionsRepositoryTests
{
    private readonly Mock<ILogger<TransactionsRepository>> _loggerMock = new();

    private WalletContext CreateInMemoryDbContext(List<Transaction>? initialTransactions = null,
        List<Category>? initialCategories = null)
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

        if (initialTransactions != null && initialTransactions.Any())
        {
            context.Transactions.AddRange(initialTransactions);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task AddTransaction_ShouldAddTransactionToDatabase()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var transaction = new Transaction("Test Transaction", 100.50m, DateOnly.FromDateTime(DateTime.UtcNow),
            TransactionTypes.Expense) { UserId = userId };

        // Act
        await repository.AddTransaction(transaction, CancellationToken.None);

        // Assert
        var addedTransaction = await context.Transactions.FirstOrDefaultAsync(t => t.Name == "Test Transaction");
        Assert.NotNull(addedTransaction);
        Assert.Equal("Test Transaction", addedTransaction.Name);
        Assert.Equal(100.50m, addedTransaction.Amount);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), addedTransaction.Date);
        Assert.Equal(TransactionTypes.Expense, addedTransaction.Type);
        Assert.Equal(userId, addedTransaction.UserId);
    }

    [Fact]
    public async Task GetAllTransactionsByType_ShouldReturnTransactionsForGivenType()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", TransactionTypes.Expense) { Id = categoryId, UserId = userId };
        var transactions = new List<Transaction>
        {
            new("Test Transaction 1", 50.00m, DateOnly.FromDateTime(DateTime.UtcNow), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Test Transaction 2", 2000.00m, DateOnly.FromDateTime(DateTime.UtcNow), TransactionTypes.Income)
                { UserId = userId },
            new("Test Transaction 3", 30.00m, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                TransactionTypes.Expense) { UserId = userId, CategoryId = categoryId, Category = category }
        };
        var context = CreateInMemoryDbContext(transactions, new List<Category> { category });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result =
            await repository.GetAllTransactionsByType(userId, TransactionTypes.Expense, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(TransactionTypes.Expense, t.Type));
        Assert.All(result, t => Assert.NotNull(t.Category));
    }

    [Fact]
    public async Task GetAllTransactionsByType_ShouldReturnEmptyList_WhenNoTransactionsForType()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result =
            await repository.GetAllTransactionsByType(userId, TransactionTypes.Expense, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTransactionsByCategory_ShouldReturnTransactionsForGivenCategory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var category1 = new Category("Test Category 1", TransactionTypes.Expense) { Id = categoryId1, UserId = userId };
        var category2 = new Category("Test Category 2", TransactionTypes.Expense) { Id = categoryId2, UserId = userId };
        var transactions = new List<Transaction>
        {
            new("Test Transaction 1", 50.00m, DateOnly.FromDateTime(DateTime.UtcNow), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId1 },
            new("Test Transaction 2", 20.00m, DateOnly.FromDateTime(DateTime.UtcNow), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId2 },
            new("Test Transaction 3", 30.00m, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                TransactionTypes.Expense) { UserId = userId, CategoryId = categoryId1 }
        };
        var context = CreateInMemoryDbContext(transactions, new List<Category> { category1, category2 });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllTransactionsByCategory(categoryId1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(categoryId1, t.CategoryId));
    }

    [Fact]
    public async Task GetAllTransactionsByCategory_ShouldThrowNotFoundException_WhenNoTransactionsForCategory()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var categoryId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            repository.GetAllTransactionsByCategory(categoryId, CancellationToken.None));
    }

    [Fact]
    public async Task GetTransactionById_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transaction =
            new Transaction("Test Transaction", 75.00m, DateOnly.FromDateTime(DateTime.UtcNow),
                TransactionTypes.Expense) { Id = transactionId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Transaction> { transaction });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetTransactionById(transactionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal("Test Transaction", result.Name);
    }

    [Fact]
    public async Task GetTransactionById_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var transactionId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            repository.GetTransactionById(transactionId, CancellationToken.None));
    }

    [Fact]
    public async Task GetTransactionByName_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transaction = new Transaction("Test Transaction", 1200.00m, DateOnly.FromDateTime(DateTime.UtcNow),
            TransactionTypes.Expense) { UserId = userId };
        var context = CreateInMemoryDbContext(new List<Transaction> { transaction });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetTransactionByName(userId, "Test Transaction", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Transaction", result.Name);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetTransactionByName_ShouldThrowNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var transactionName = "NonExisting Transaction";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            repository.GetTransactionByName(userId, transactionName, CancellationToken.None));
    }

    [Fact]
    public async Task GetTransactionsByPeriod_ShouldReturnTransactionsWithinGivenPeriod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", TransactionTypes.Expense) { Id = categoryId, UserId = userId };
        var startDate = new DateOnly(2025, 3, 15);
        var endDate = new DateOnly(2025, 3, 25);
        var transactions = new List<Transaction>
        {
            new("Item 1", 10.00m, new DateOnly(2025, 3, 10), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Item 2", 20.00m, new DateOnly(2025, 3, 18), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Item 3", 30.00m, new DateOnly(2025, 3, 22), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Item 4", 40.00m, new DateOnly(2025, 3, 28), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category }
        };
        var context = CreateInMemoryDbContext(transactions, new List<Category> { category });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetTransactionsByPeriod(userId, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.True(t.Date >= startDate && t.Date <= endDate));
        Assert.All(result, t => Assert.NotNull(t.Category));
    }

    [Fact]
    public async Task GetTransactionsByPeriod_ShouldReturnEmptyList_WhenNoTransactionsInPeriod()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var startDate = new DateOnly(2025, 4, 1);
        var endDate = new DateOnly(2025, 4, 10);

        // Act
        var result = await repository.GetTransactionsByPeriod(userId, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsByTypeAndPeriod_ShouldReturnTransactionsForGivenTypeAndPeriod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category", TransactionTypes.Expense) { Id = categoryId, UserId = userId };
        var startDate = new DateOnly(2025, 3, 15);
        var endDate = new DateOnly(2025, 3, 25);
        var transactions = new List<Transaction>
        {
            new("Income 1", 100.00m, new DateOnly(2025, 3, 18), TransactionTypes.Income) 
                { UserId = userId },
            new("Expense 1", 20.00m, new DateOnly(2025, 3, 20), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Expense 2", 30.00m, new DateOnly(2025, 3, 23), TransactionTypes.Expense)
                { UserId = userId, CategoryId = categoryId, Category = category },
            new("Income 2", 150.00m, new DateOnly(2025, 3, 28), TransactionTypes.Income) 
                { UserId = userId }
        };
        var context = CreateInMemoryDbContext(transactions, new List<Category> { category });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetTransactionsByTypeAndPeriod(userId, TransactionTypes.Expense, startDate,
            endDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result,
            t => Assert.True(t.Date >= startDate && t.Date <= endDate && t.Type == TransactionTypes.Expense));
        Assert.All(result, t => Assert.NotNull(t.Category));
    }

    [Fact]
    public async Task GetTransactionsByTypeAndPeriod_ShouldReturnEmptyList_WhenNoTransactionsForTypeAndPeriod()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new TransactionsRepository(context, _loggerMock.Object);
        var userId = Guid.NewGuid();
        var startDate = new DateOnly(2025, 4, 1);
        var endDate = new DateOnly(2025, 4, 10);

        // Act
        var result = await repository.GetTransactionsByTypeAndPeriod(userId, TransactionTypes.Expense, startDate,
            endDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldUpdateExistingTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTransaction =
            new Transaction("Old Name", 50.00m, new DateOnly(2025, 3, 20), TransactionTypes.Expense)
                { Id = transactionId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Transaction> { existingTransaction });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        var transactionToUpdate = await context.Transactions.FindAsync(transactionId);
        Assert.NotNull(transactionToUpdate);
        transactionToUpdate.Name = "Updated Name";
        transactionToUpdate.Amount = 75.00m;
        transactionToUpdate.Date = new DateOnly(2025, 3, 25);
        transactionToUpdate.Type = TransactionTypes.Income;

        await repository.UpdateTransaction(transactionToUpdate, CancellationToken.None);

        // Assert
        var transactionFromDb = await context.Transactions.FindAsync(transactionId);
        Assert.NotNull(transactionFromDb);
        Assert.Equal("Updated Name", transactionFromDb.Name);
        Assert.Equal(75.00m, transactionFromDb.Amount);
        Assert.Equal(new DateOnly(2025, 3, 25), transactionFromDb.Date);
        Assert.Equal(TransactionTypes.Income, transactionFromDb.Type);
    }

    [Fact]
    public async Task DeleteTransaction_ShouldRemoveExistingTransaction()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionToDelete =
            new Transaction("To Delete", 25.00m, new DateOnly(2025, 3, 20), TransactionTypes.Expense)
                { Id = transactionId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Transaction> { transactionToDelete });
        var repository = new TransactionsRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteTransaction(transactionToDelete, CancellationToken.None);

        // Assert
        var transactionFromDb = await context.Transactions.FindAsync(transactionId);
        Assert.Null(transactionFromDb);
    }
}