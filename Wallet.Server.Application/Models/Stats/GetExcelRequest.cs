namespace Wallet.Server.Application.Models.Stats;

public record GetExcelRequest(Guid UserId, string Period);