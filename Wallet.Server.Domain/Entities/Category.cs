using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Entities;

public class Category(Guid userId, string name, TransactionTypes type)
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; } = userId;
    public string Name { get; set; } = name;
    public TransactionTypes Type { get; set; } = type;
}