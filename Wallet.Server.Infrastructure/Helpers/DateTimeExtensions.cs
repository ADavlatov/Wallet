namespace Wallet.Server.Infrastructure.Helpers;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (dt.DayOfWeek - startOfWeek + 7) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}