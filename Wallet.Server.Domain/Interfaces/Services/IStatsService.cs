using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IStatsService
{
    public Task<byte[]> GenerateExcelFile(Guid userId, CancellationToken cancellationToken);
    public Task<LineChartDto> GetLineChartData(Guid userId, string period, CancellationToken cancellationToken);
    Task<Dictionary<string, decimal>> GetPieChartData(Guid userId, TransactionTypes type, string period, CancellationToken cancellationToken);
}