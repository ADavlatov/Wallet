using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Stats;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

/// <summary>
/// Контроллер для работы со статистикой
/// </summary>
/// <param name="statsService">Сервис статистики</param>
[Authorize]
[ApiController]
[Route("api/v1/stats")]
public class StatsController(
    IStatsService statsService,
    ILogger<StatsController> logger,
    IValidator<GetExcelRequest> getExcelRequestValidator,
    IValidator<GetLineChartDataRequest> getLineChartDataRequestValidator,
    IValidator<GetPieChartRequest> getPieChartRequestValidator) : ControllerBase
{
    /// <summary>
    /// Получает Excel файл с транзакциями
    /// </summary>
    /// <param name="request">Данные для получения файла с транзакциями. Содержит UserId и Period</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Файл Excel в формате xlsx</returns>
    [HttpPost("GetExcelFile")]
    public async Task<IActionResult> GetExcelFile([FromBody] GetExcelRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на получение Excel файла с транзакциями. UserId: {request.UserId}, Period: {request.Period}.");
        var validationResult = await getExcelRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при запросе Excel файла. UserId: {request.UserId}, Period: {request.Period}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var excelFileBytes = await statsService.GenerateExcelFile(request.UserId, request.Period, cancellationToken);
        string fileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        logger.LogInformation(
            $"Запрос на получение Excel файла с транзакциями завершен. UserId: {request.UserId}, Period: {request.Period}. Имя файла: {fileName}.");
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
        logger.LogInformation(
            $"Начало запроса на получение данных для линейного графика. UserId: {request.UserId}, Period: {request.Period}.");
        var validationResult = await getLineChartDataRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при запросе данных для линейного графика. UserId: {request.UserId}, Period: {request.Period}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var result = await statsService.GetLineChartData(request.UserId, request.Period, cancellationToken);
        logger.LogInformation(
            $"Запрос на получение данных для линейного графика завершен. UserId: {request.UserId}, Period: {request.Period}.");
        return Ok(result);
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
        logger.LogInformation(
            $"Начало запроса на получение данных для кругового графика. UserId: {request.UserId}, Type: {request.type}, Period: {request.Period}.");
        var validationResult = await getPieChartRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при запросе данных для кругового графика. UserId: {request.UserId}, Type: {request.type}, Period: {request.Period}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var data = await statsService.GetPieChartData(request.UserId, request.type, request.Period,
            cancellationToken);
        logger.LogInformation(
            $"Запрос на получение данных для кругового графика завершен. UserId: {request.UserId}, Type: {request.type}, Period: {request.Period}.");
        return Ok(data);
    }
}