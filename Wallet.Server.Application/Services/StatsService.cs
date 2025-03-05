using ClosedXML.Excel;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Interfaces.Repositories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Application.Services;

public class StatsService(ITransactionsRepository transactionsRepository) : IStatsService
{
    public async Task<byte[]> GenerateExcelFile(Guid userId, CancellationToken cancellationToken)
    {
        var incomes = await transactionsRepository.GetAllTransactionsByType(userId, TransactionTypes.Income, cancellationToken); // Получаем доходы
        var expenses = await transactionsRepository.GetAllTransactionsByType(userId, TransactionTypes.Expense, cancellationToken); // Получаем расходы

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
                worksheet.Cell(incomeRow, 1).Value = transaction.Date;
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
                worksheet.Cell(expenseRow, 5).Value = transaction.Date;
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
}