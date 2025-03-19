using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Application.Models.Stats;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/stats")]
public class StatsController(IStatsService statsService) : ControllerBase
{
    [HttpPost("GetExcelFile")]
    public async Task<IActionResult> GetExcelFile([FromBody] GetExcelRequest request, CancellationToken cancellationToken)
    {
        var excelFileBytes = await statsService.GenerateExcelFile(request.UserId, request.period, cancellationToken);
        string fileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("GetLineChartData")]
    public async Task<IActionResult> GetLineChartData([FromBody] GetLineChartDataRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await statsService.GetLineChartData(request.UserId, request.Period, cancellationToken));
    }

    [HttpPost("GetPieChartData")]
    public async Task<IActionResult> GetExpensePieChartData([FromBody] GetPieChartRequest request,
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