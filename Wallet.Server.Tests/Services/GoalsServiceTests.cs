using Microsoft.Extensions.Logging;
using Moq;
using Wallet.Server.Application.Services;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces.Repositories;

namespace Wallet.Server.Tests.Services;

public class GoalsServiceTests
{
    private readonly Mock<IGoalsRepository> _goalsRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GoalsService _goalsService;

    public GoalsServiceTests()
    {
        _goalsRepositoryMock = new Mock<IGoalsRepository>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        Mock<ILogger<GoalsService>> loggerMock = new();
        _goalsService = new GoalsService(_goalsRepositoryMock.Object, _usersRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task AddGoal_ShouldAddGoalToRepository_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Save for a car";
        var amount = 10000;
        var deadline = new DateOnly(2025, 12, 31);
        var user = new User("testuser", [], [], "testkey") { Id = userId };
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _goalsService.AddGoal(userId, name, amount, deadline, CancellationToken.None);

        // Assert
        _goalsRepositoryMock.Verify(repo => repo.AddGoal(
            It.Is<Goal>(g =>
                g.Name == name && g.TargetSum == amount && g.Deadline == deadline && g.UserId == userId &&
                g.User == user),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddGoal_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Save for a car";
        var amount = 10000;
        var deadline = new DateOnly(2025, 12, 31);
        _usersRepositoryMock.Setup(repo => repo.GetUserById(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _goalsService.AddGoal(userId, name, amount, deadline, CancellationToken.None));

        _goalsRepositoryMock.Verify(repo => repo.AddGoal(It.IsAny<Goal>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddSumToGoal_ShouldCallRepositoryAddSumToGoal_WhenGoalExists()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var sumToAdd = 500;
        var existingGoal = new Goal("Laptop", 1500) { Id = goalId, CurrentSum = 200 };
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGoal);

        // Act
        await _goalsService.AddSumToGoal(goalId, sumToAdd, CancellationToken.None);

        // Assert
        _goalsRepositoryMock.Verify(repo => repo.AddSumToGoal(existingGoal, sumToAdd, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddSumToGoal_ShouldThrowNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var sumToAdd = 500;
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Goal not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _goalsService.AddSumToGoal(goalId, sumToAdd, CancellationToken.None));

        _goalsRepositoryMock.Verify(
            repo => repo.AddSumToGoal(It.IsAny<Goal>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetGoalsByUserId_ShouldReturnGoalsFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var goals = new List<Goal>
        {
            new("Car", 10000),
            new("Vacation", 2000)
        };
        _goalsRepositoryMock.Setup(repo => repo.GetAllGoalsByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goals);

        // Act
        var result = await _goalsService.GetGoalsByUserId(userId, CancellationToken.None);

        // Assert
        Assert.Equal(goals, result);
    }

    [Fact]
    public async Task GetGoalById_ShouldReturnGoalFromRepository()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var goal = new Goal("Laptop", 1500);
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goal);

        // Act
        var result = await _goalsService.GetGoalById(goalId, CancellationToken.None);

        // Assert
        Assert.Equal(goal, result);
    }

    [Fact]
    public async Task UpdateGoal_ShouldUpdateGoalInRepository_WhenGoalExistsAndPropertiesAreProvided()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var existingGoal = new Goal("Old Name", 1000) { Id = goalId, Deadline = new DateOnly(2024, 10, 20) };
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGoal);
        var newName = "New Name";
        var newAmount = 2000;
        var newDeadline = new DateOnly(2025, 11, 25);

        // Act
        await _goalsService.UpdateGoal(goalId, newName, newAmount, newDeadline, CancellationToken.None);

        // Assert
        Assert.Equal(newName, existingGoal.Name);
        Assert.Equal(newAmount, existingGoal.TargetSum);
        Assert.Equal(newDeadline, existingGoal.Deadline);
        _goalsRepositoryMock.Verify(repo => repo.UpdateGoal(existingGoal, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGoal_ShouldUpdateGoalInRepository_WhenGoalExistsAndSomePropertiesAreNull()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var existingGoal = new Goal("Old Name", 1000) { Id = goalId, Deadline = new DateOnly(2024, 10, 20) };
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingGoal);
        string? newName = null;
        decimal? newAmount = null;
        DateOnly? newDeadline = null;

        // Act
        await _goalsService.UpdateGoal(goalId, newName, newAmount, newDeadline, CancellationToken.None);

        // Assert
        Assert.Equal("Old Name", existingGoal.Name);
        Assert.Equal(1000, existingGoal.TargetSum);
        Assert.Equal(new DateOnly(2024, 10, 20), existingGoal.Deadline);
        _goalsRepositoryMock.Verify(repo => repo.UpdateGoal(existingGoal, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGoal_ShouldThrowNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Goal not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _goalsService.UpdateGoal(goalId, "New Name", 2000, new DateOnly(2025, 11, 25), CancellationToken.None));

        _goalsRepositoryMock.Verify(repo => repo.UpdateGoal(It.IsAny<Goal>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteGoal_ShouldDeleteGoalFromRepository_WhenGoalExists()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        var goalToDelete = new Goal("Car", 10000) { Id = goalId };
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goalToDelete);

        // Act
        await _goalsService.DeleteGoal(goalId, CancellationToken.None);

        // Assert
        _goalsRepositoryMock.Verify(repo => repo.DeleteGoal(goalToDelete, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteGoal_ShouldThrowNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var goalId = Guid.NewGuid();
        _goalsRepositoryMock.Setup(repo => repo.GetGoalById(goalId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Goal not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _goalsService.DeleteGoal(goalId, CancellationToken.None));

        _goalsRepositoryMock.Verify(repo => repo.DeleteGoal(It.IsAny<Goal>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}