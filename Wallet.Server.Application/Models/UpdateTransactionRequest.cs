namespace Wallet.Server.Application.Models;

public record UpdateTransactionRequest(Guid TransactionId, string? Name, decimal? Amount, DateTime? Date, string? Type);