using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Transaction(string? name, decimal amount, DateOnly date, TransactionTypes type)
{
    public Guid Id { get; init; }
    public User User { get; init; }
    public Guid UserId { get; init; }
    public Category Category { get; set; }
    public Guid CategoryId { get; set; }
    public string? Name { get; set; } = name;
    public decimal Amount { get; set; } = amount;
    public DateOnly Date { get; set; } = date;
    public TransactionTypes Type { get; set; } = type;
}