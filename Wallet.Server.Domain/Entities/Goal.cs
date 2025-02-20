namespace Wallet.Server.Domain.Entities;

public class Goal(Guid userId, string name, decimal amount, DateOnly? deadline = null)
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; } = userId;
    public string Name { get; set; } = name;
    public Decimal Amount { get; set; } = amount;
    public DateOnly? Deadline { get; set; } = deadline;
}