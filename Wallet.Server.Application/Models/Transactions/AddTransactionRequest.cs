namespace Wallet.Server.Application.Models.Transactions;

public record AddTransactionRequest(Guid UserId, Guid CategoryId, string? Name, decimal Amount, DateTime Date, string Type);