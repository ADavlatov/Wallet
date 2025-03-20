using Microsoft.EntityFrameworkCore;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Infrastructure.Contexts;
using Wallet.Server.Infrastructure.Repositories;

namespace Wallet.Server.Tests.Repositories;

public class GoalsRepositoryTests
{
    private WalletContext CreateInMemoryDbContext(List<Goal>? initialGoals = null)
    {
        var options = new DbContextOptionsBuilder<WalletContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new WalletContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (initialGoals != null && initialGoals.Any())
        {
            context.Goals.AddRange(initialGoals);
            context.SaveChanges();
        }

        return context;
    }

    [Fact]
    public async Task AddGoal_ShouldAddGoalToDatabase()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new GoalsRepository(context);
        var goal = new Goal("Test Goal", 10000) { UserId = Guid.NewGuid() };

        // Act
        await repository.AddGoal(goal, CancellationToken.None);

        // Assert
        var addedGoal = await context.Goals.FirstOrDefaultAsync(g => g.Name == "Test Goal");
        Assert.NotNull(addedGoal);
        Assert.Equal("Test Goal", addedGoal.Name);
        Assert.Equal(10000, addedGoal.TargetSum);
    }

    [Fact]
    public async Task AddSumtoGoal_ShouldIncreaseCurrentSum()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var existingGoal = new Goal("Test Goal", 5000) { Id = goalId, UserId = Guid.NewGuid(), CurrentSum = 1000 };
        var context = CreateInMemoryDbContext(new List<Goal> { existingGoal });
        var repository = new GoalsRepository(context);
        decimal amountToAdd = 500;

        // Act
        var goalToUpdate = await context.Goals.FindAsync(goalId);
        Assert.NotNull(goalToUpdate);
        await repository.AddSumtoGoal(goalToUpdate, amountToAdd, CancellationToken.None);

        // Assert
        var updatedGoal = await context.Goals.FindAsync(goalId);
        Assert.NotNull(updatedGoal);
        Assert.Equal(1500, updatedGoal.CurrentSum);
    }

    [Fact]
    public async Task GetAllGoalsByUserId_ShouldReturnGoalsForGivenUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var goals = new List<Goal>
        {
            new("Test Goal 1", 10000) { UserId = userId1 },
            new("Test Goal 2", 100000) { UserId = userId1 },
            new("Test Goal 3", 500) { UserId = userId2 }
        };
        var context = CreateInMemoryDbContext(goals);
        var repository = new GoalsRepository(context);

        // Act
        var result = await repository.GetAllGoalsByUserId(userId1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, g => Assert.Equal(userId1, g.UserId));
    }

    [Fact]
    public async Task GetAllGoalsByUserId_ShouldReturnEmptyList_WhenNoGoalsForUser()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new GoalsRepository(context);
        var userId = Guid.NewGuid();

        // Act
        var result = await repository.GetAllGoalsByUserId(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGoalById_ShouldReturnGoal_WhenGoalExists()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var goal = new Goal("Test Goal", 1500) { Id = goalId, UserId = Guid.NewGuid() };
        var context = CreateInMemoryDbContext(new List<Goal> { goal });
        var repository = new GoalsRepository(context);

        // Act
        var result = await repository.GetGoalById(goalId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(goalId, result.Id);
        Assert.Equal("Test Goal", result.Name);
    }

    [Fact]
    public async Task GetGoalById_ShouldThrowNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new GoalsRepository(context);
        var goalId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetGoalById(goalId, CancellationToken.None));
    }

    [Fact]
    public async Task GetGoalByName_ShouldReturnGoal_WhenGoalExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var goal = new Goal("Test Goal", 3000) { UserId = userId };
        var context = CreateInMemoryDbContext(new List<Goal> { goal });
        var repository = new GoalsRepository(context);

        // Act
        var result = await repository.GetGoalByName(userId, "Test Goal", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Goal", result.Name);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetGoalByName_ShouldThrowNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var context = CreateInMemoryDbContext();
        var repository = new GoalsRepository(context);
        var userId = Guid.NewGuid();
        var goalName = "NonExisting Goal";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetGoalByName(userId, goalName, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateGoal_ShouldUpdateExistingGoal()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingGoal = new Goal("Old Goal", 5000) { Id = goalId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Goal> { existingGoal });
        var repository = new GoalsRepository(context);

        // Act
        var goalToUpdate = await context.Goals.FindAsync(goalId);
        Assert.NotNull(goalToUpdate);
        goalToUpdate.Name = "New Goal";
        goalToUpdate.TargetSum = 7000;

        await repository.UpdateGoal(goalToUpdate, CancellationToken.None);

        // Assert
        var goalFromDb = await context.Goals.FindAsync(goalId);
        Assert.NotNull(goalFromDb);
        Assert.Equal("New Goal", goalFromDb.Name);
        Assert.Equal(7000, goalFromDb.TargetSum);
    }

    [Fact]
    public async Task DeleteGoal_ShouldRemoveExistingGoal()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var goalToDelete = new Goal("To Delete", 1000) { Id = goalId, UserId = userId };
        var context = CreateInMemoryDbContext(new List<Goal> { goalToDelete });
        var repository = new GoalsRepository(context);

        // Act
        await repository.DeleteGoal(goalToDelete, CancellationToken.None);

        // Assert
        var goalFromDb = await context.Goals.FindAsync(goalId);
        Assert.Null(goalFromDb);
    }
}