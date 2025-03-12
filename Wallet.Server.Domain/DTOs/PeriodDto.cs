namespace Wallet.Server.Domain.DTOs;

public record PeriodDto(DateOnly StartDate, DateOnly EndDate, int PeriodLength);