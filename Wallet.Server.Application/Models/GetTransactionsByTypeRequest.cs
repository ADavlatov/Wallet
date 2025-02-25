namespace Wallet.Server.Application.Models;

public record GetTransactionsByTypeRequest(Guid UserId, string Type);