namespace Wallet.Server.Application.Models.Transactions;

public record GetTransactionsByTypeRequest(Guid UserId, string Type);