using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Category(User user, string name, TransactionTypes type)
{
    public Guid Id { get; init; }
    public User User { get; init; } = user;
    public Guid UserId { get; init; } = user.Id;
    public string Name { get; set; } = name;
    public TransactionTypes Type { get; set; } = type;
    public List<Transaction> Transactions { get; set; } = new();
}