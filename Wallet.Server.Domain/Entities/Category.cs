using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Category(string name, TransactionTypes type)
{
    public Guid Id { get; init; }
    public User User { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; set; } = name;
    public TransactionTypes Type { get; set; } = type;
    public List<Transaction> Transactions { get; set; } = new();
}