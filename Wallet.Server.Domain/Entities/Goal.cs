namespace Wallet.Server.Domain.Entities;

public class Goal(string name, decimal targetSum, DateOnly? deadline = null)
{
    public Guid Id { get; init; }
    public User User { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; set; } = name;
    public decimal TargetSum { get; set; } = targetSum;
    public decimal CurrentSum { get; set; }
    public DateOnly? Deadline { get; set; } = deadline;
}