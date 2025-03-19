using Wallet.Server.Domain.DTOs;

namespace Wallet.Server.Infrastructure.Helpers;

public static class PeriodHelper
{
    public static PeriodDto GetPeriodDates(string period)
    {
        DateTime now = DateTime.Now;
        DateTime startDate, endDate;
        int periodLength;

        switch (period.ToLower())
        {
            case "week":
                startDate = now.StartOfWeek(DayOfWeek.Monday);
                endDate = startDate.AddDays(6);
                periodLength = 7;
                break;
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                periodLength = DateTime.DaysInMonth(now.Year, now.Month);
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
                periodLength = 365;
                break;
            default:
                throw new ArgumentException("Неверный период: " + period);
        }

        return new PeriodDto(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), periodLength);
    }
}