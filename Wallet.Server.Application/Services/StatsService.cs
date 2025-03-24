using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.DTOs;
using Wallet.Server.Domain.Entities;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;
using Wallet.Server.Infrastructure.Helpers;

namespace Wallet.Server.Application.Services;

public class StatsService(ITransactionsRepository transactionsRepository, ILogger<StatsService> logger) : IStatsService
{
    public async Task<byte[]> GenerateExcelFile(Guid userId, string period, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на генерацию Excel файла. UserId: {UserId}, Period: {Period}", userId, period);
        var periodDates = PeriodHelper.GetPeriodDates(period, logger);
        var transactions = await transactionsRepository
            .GetTransactionsByPeriod(userId, periodDates.StartDate, periodDates.EndDate, cancellationToken);

        var workbook = CreateWorkbook(transactions);
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return content;
    }

    public async Task<LineChartDto> GetLineChartData(Guid userId, string period, CancellationToken cancellationToken)
    {
        logger.LogInformation("Запрос на получение данных для линейного графика. " +
                              "UserId: {UserId}, Period: {Period}", userId, period);

        var periodDates = PeriodHelper.GetPeriodDates(period, logger);
        var transactions = await transactionsRepository
            .GetTransactionsByPeriod(userId, periodDates.StartDate, periodDates.EndDate, cancellationToken);

        decimal[] incomes = new decimal[periodDates.PeriodLength];
        decimal[] expenses = new decimal[periodDates.PeriodLength];

        for (int i = 0; i < periodDates.PeriodLength; i++)
        {
            var currentDate = periodDates.StartDate.AddDays(i);
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
        logger.LogInformation("Запрос на получение данных для кругового графика. " +
                              "UserId: {UserId}, Type: {Type}, Period: {Period}", userId, type, period);

        var periodDates = PeriodHelper.GetPeriodDates(period, logger);
        var expenses = await transactionsRepository
            .GetTransactionsByTypeAndPeriod(userId, type, periodDates.StartDate, periodDates.EndDate,
                cancellationToken);

        return expenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key.Name, g => g.Sum(e => e.Amount));
    }

    private XLWorkbook CreateWorkbook(List<Transaction> transactions)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Доходы");

        worksheet.Cell(1, 1).Value = "Дата";
        worksheet.Cell(1, 2).Value = "Категория";
        worksheet.Cell(1, 3).Value = "Сумма";

        var incomeHeadersRange = worksheet.Range("A1:C1");
        incomeHeadersRange.Style.Font.Bold = true;
        incomeHeadersRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var incomes = transactions.Where(x => x.Type == TransactionTypes.Income).ToList();
        int incomeRow = 2;
        if (incomes.Any())
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

        var expenses = transactions.Where(x => x.Type == TransactionTypes.Expense).ToList();
        int expenseRow = 2;
        if (expenses.Any())
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

        return workbook;
    }
}