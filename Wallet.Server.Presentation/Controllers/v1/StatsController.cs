using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/stats")]
public class StatsController(IStatsService statsService) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetExcelFile(Guid userId, CancellationToken cancellationToken)
    {
        var excelFileBytes = await statsService.GenerateExcelFile(userId, cancellationToken);
        string fileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}