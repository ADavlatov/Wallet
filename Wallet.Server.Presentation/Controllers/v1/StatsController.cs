using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Stats;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/stats")]
public class StatsController(IStatsService statsService) : ControllerBase
{
   
    /// <summary>
    /// Получает Excel файл с транзакциями
    /// </summary>
    /// <param name="request">Данные для получения файла с транзакциями. Содержит UserId и Period</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Файл Excel в формате xlsx</returns>
    [HttpPost("GetExcelFile")]
    public async Task<IActionResult> GetExcelFile([FromBody] GetExcelRequest request, CancellationToken cancellationToken)
    {
        var excelFileBytes = await statsService.GenerateExcelFile(request.UserId, request.Period, cancellationToken);
        string fileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    
    /// <summary>
    /// Получает данные для построения линейного графика
    /// </summary>
    /// <param name="request">Данные для получения данных для линейного графика. Содержит UserId и Period</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные для линейного графика</returns>
    [HttpPost("GetLineChartData")]
    public async Task<IActionResult> GetLineChartData([FromBody] GetLineChartDataRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await statsService.GetLineChartData(request.UserId, request.Period, cancellationToken));
    }

    
    /// <summary>
    /// Получает данные для построения кругового графика
    /// </summary>
    /// <param name="request">Данные для получения данных для кругового графика. Содержит UserId, TransactionType и Period</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные для кругового графика</returns>
    [HttpPost("GetPieChartData")]
    public async Task<IActionResult> GetPieChartData([FromBody] GetPieChartRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await statsService.GetPieChartData(request.UserId, request.type, request.Period,
                cancellationToken);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}