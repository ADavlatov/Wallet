namespace Wallet.Server.Domain.Entities;

public class Goal(string name, decimal amount, DateOnly? deadline = null)
{
    public Guid Id { get; init; }
    public User User { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; set; } = name;
    public decimal Amount { get; set; } = amount;
    public decimal Value { get; set; } = 0;
    public DateOnly? Deadline { get; set; } = deadline;
}