using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Transaction(Guid userId, Guid categoryId, string? name, decimal amount, DateTime date, TransactionTypes type)
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; } = userId;
    public Guid CategoryId { get; init; } = categoryId;
    public string? Name { get; set; } = name;
    public decimal Amount { get; set; } = amount;
    public DateTime Date { get; set; } = date;
    public TransactionTypes Type { get; set; } = type;
}