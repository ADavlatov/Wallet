using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Application.Models.Stats;

public record GetPieChartRequest(Guid UserId, TransactionTypes type, string Period);