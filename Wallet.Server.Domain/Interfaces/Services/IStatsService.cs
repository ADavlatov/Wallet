using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Domain.Interfaces.Services;

public interface IStatsService
{
    public Task<byte[]> GenerateExcelFile(Guid userId, CancellationToken cancellationToken);
    public Task<List<int>> GetStatsByTypAndDateInterval(Guid userId, DateOnly startDate, DateOnly endDate, TransactionTypes type, CancellationToken cancellationToken);
}