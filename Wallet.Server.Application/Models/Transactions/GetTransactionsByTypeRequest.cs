using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Application.Models.Transactions;

public record GetTransactionsByTypeRequest(Guid UserId, TransactionTypes Type);