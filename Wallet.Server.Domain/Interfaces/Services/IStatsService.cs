namespace Wallet.Server.Domain.Interfaces.Services;

public interface IStatsService
{
    public Task<byte[]> GenerateExcelFile(Guid userId, CancellationToken cancellationToken);
}