namespace Wallet.Server.Application.Models.Transactions;

public record UpdateTransactionRequest(Guid TransactionId, Guid? CategoryId, string? Name, decimal? Amount, DateOnly? Date);