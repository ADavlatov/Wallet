using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Application.Models.Transactions;

public record UpdateTransactionRequest(Guid TransactionId, string? Name, decimal? Amount, DateOnly? Date, TransactionTypes? Type);