using ClosedXML.Excel;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;
using Wallet.Server.Infrastructure.Helpers;

namespace Wallet.Server.Application.Services;

public class StatsService(ITransactionsRepository transactionsRepository) : IStatsService
{
    public async Task<byte[]> GenerateExcelFile(Guid userId, CancellationToken cancellationToken)
    {
        var incomes =
            await transactionsRepository.GetAllTransactionsByType(userId, TransactionTypes.Income,
                cancellationToken); // Получаем доходы
        var expenses =
            await transactionsRepository.GetAllTransactionsByType(userId, TransactionTypes.Expense,
                cancellationToken); // Получаем расходы

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Доходы");

        worksheet.Cell(1, 1).Value = "Дата";
        worksheet.Cell(1, 2).Value = "Категория";
        worksheet.Cell(1, 3).Value = "Сумма";

        var incomeHeadersRange = worksheet.Range("A1:C1");
        incomeHeadersRange.Style.Font.Bold = true;
        incomeHeadersRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        int incomeRow = 2;
        if (incomes.Count > 0)
        {
            foreach (var transaction in incomes)
            {
                worksheet.Cell(incomeRow, 1).Value = transaction.Date.ToString();
                worksheet.Cell(incomeRow, 1).Style.DateFormat.Format = "dd.MM.yyyy";
                worksheet.Cell(incomeRow, 2).Value = transaction.Category.Name;
                worksheet.Cell(incomeRow, 3).Value = transaction.Amount;
                incomeRow++;
            }
        }
        else
        {
            worksheet.Cell(2, 1).Value = "Нет данных о доходах";
        }


        worksheet.Cell(1, 5).Value = "Дата";
        worksheet.Cell(1, 6).Value = "Категория";
        worksheet.Cell(1, 7).Value = "Сумма";

        var expenseHeadersRange = worksheet.Range("E1:G1");
        expenseHeadersRange.Style.Font.Bold = true;
        expenseHeadersRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        int expenseRow = 2;
        if (expenses.Count > 0)
        {
            foreach (var transaction in expenses)
            {
                worksheet.Cell(expenseRow, 5).Value = transaction.Date.ToString();
                worksheet.Cell(expenseRow, 5).Style.DateFormat.Format = "dd.MM.yyyy";
                worksheet.Cell(expenseRow, 6).Value = transaction.Category.Name;
                worksheet.Cell(expenseRow, 7).Value = transaction.Amount;
                expenseRow++;
            }
        }
        else
        {
            worksheet.Cell(2, 1).Value = "Нет данных о расходах";
        }

        worksheet.Columns().AdjustToContents();
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();
        return content;
    }

    public async Task<LineChartDto> GetLineChartData(Guid userId, string period, CancellationToken cancellationToken)
    {
        (DateTime startDate, DateTime endDate) = GetPeriodDates(period);
        var transactions = await transactionsRepository.GetTransactionsByPeriod(userId,
            DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), cancellationToken);

        int periodLength = (endDate - startDate).Days + 1;
        decimal[] incomes = new decimal[periodLength];
        decimal[] expenses = new decimal[periodLength];
        var date = DateOnly.FromDateTime(startDate);

        for (int i = 0; i < periodLength; i++)
        {
            var currentDate = date.AddDays(i);
            incomes[i] = transactions
                .Where(t => t.Type == TransactionTypes.Income && t.Date == currentDate)
                .Sum(t => t.Amount);

            expenses[i] = transactions
                .Where(t => t.Type == TransactionTypes.Expense && t.Date == currentDate)
                .Sum(t => t.Amount);
        }

        return new LineChartDto(incomes, expenses);
    }

    public async Task<Dictionary<string, decimal>> GetPieChartData(Guid userId, TransactionTypes type, string period,
        CancellationToken cancellationToken)
    {
        (DateTime startDate, DateTime endDate) = GetPeriodDates(period);
        var expenses = await transactionsRepository.GetTransactionsByTypeAndPeriod(userId, type,
            DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), cancellationToken);

        return expenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key.Name, g => g.Sum(e => e.Amount));
    }

    private (DateTime, DateTime) GetPeriodDates(string period)
    {
        DateTime now = DateTime.Now;
        DateTime startDate, endDate;

        switch (period.ToLower())
        {
            case "week":
                startDate = now.StartOfWeek(DayOfWeek.Monday);
                endDate = startDate.AddDays(6);
                break;
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
                break;
            default:
                throw new ArgumentException("Неверный период: " + period);
        }

        return (startDate, endDate);
    }
}