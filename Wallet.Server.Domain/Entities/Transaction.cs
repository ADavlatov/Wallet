using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Transaction(User user, Category category, string? name, decimal amount, DateTime date, TransactionTypes type)
{
    public Guid Id { get; init; }
    public User User { get; init; } = user;
    public Guid UserId { get; init; } = user.Id;
    public Category Category { get; init; } = category;
    public Guid CategoryId { get; init; } = category.Id;
    public string? Name { get; set; } = name;
    public decimal Amount { get; set; } = amount;
    public DateTime Date { get; set; } = date;
    public TransactionTypes Type { get; set; } = type;
}