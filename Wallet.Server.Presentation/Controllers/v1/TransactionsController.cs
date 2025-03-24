using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Transactions;
using Wallet.Server.Application.Validators.Transactions;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

/// <summary>
/// Контроллер для работы с транзакциями
/// </summary>
/// <param name="transactionsService">Сервис транзакций</param>
[Authorize]
[ApiController]
[Route("/api/v1/transactions")]
public class TransactionsController(
    ITransactionsService transactionsService,
    ILogger<TransactionsController> logger) : ControllerBase
{
    /// <summary>
    /// Добавляет новую транзакцию.
    /// </summary>
    /// <param name="request">Данные для добавления транзакции. Содержит UserId, CategoryId, Name nullable, Amount, Date и TransactionType</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("AddTransaction")]
    public async Task<IActionResult> AddTransaction([FromBody] AddTransactionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на добавление транзакции. UserId: {request.UserId}, CategoryId: {request.CategoryId}, Type: {request.Type}, Amount: {request.Amount}, Date: {request.Date}.");
        var validationResult = await new AddTransactionRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при добавлении транзакции. UserId: {request.UserId}, CategoryId: {request.CategoryId}, Type: {request.Type}, Amount: {request.Amount}, Date: {request.Date}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        await transactionsService.AddTransaction(request.UserId, request.CategoryId, request.Name, request.Amount,
            request.Date, request.Type, cancellationToken);
        logger.LogInformation(
            $"Запрос на добавление транзакции завершен. UserId: {request.UserId}, CategoryId: {request.CategoryId}.");
        return Ok();
    }

    /// <summary>
    /// Получает список транзакций по типу.
    /// </summary>
    /// <param name="request">Данные для получения транзакции. Содержит UserId и TransactionType</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список транзакций</returns>
    [HttpPost("GetTransactionsByType")]
    public async Task<IActionResult> GetTransactionsByType([FromBody] GetTransactionsByTypeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на получение транзакций по типу. UserId: {request.UserId}, Type: {request.Type}.");
        var result = await transactionsService.GetTransactionsByType(request.UserId, request.Type, cancellationToken);
        logger.LogInformation(
            $"Запрос на получение транзакций по типу завершен. UserId: {request.UserId}, Type: {request.Type}. Количество транзакций: {result.Count}.");
        return Ok(result);
    }

    /// <summary>
    /// Получает список транзакций по категории.
    /// </summary>
    /// <param name="request">Данные для получения транзакции. Содержит CategoryId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список транзакций</returns>
    [HttpPost("GetTransactionsByCategory")]
    public async Task<IActionResult> GetTransactionsByCategory([FromBody] GetTransactionsByCategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на получение транзакций по категории. CategoryId: {request.CategoryId}.");
        var result = await transactionsService.GetTransactionsByCategory(request.CategoryId, cancellationToken);
        logger.LogInformation(
            $"Запрос на получение транзакций по категории завершен. CategoryId: {request.CategoryId}. Количество транзакций: {result.Count}.");
        return Ok(result);
    }

    /// <summary>
    /// Получает транзакцию по идентификатору.
    /// </summary>
    /// <param name="transactionId">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Модель транзакции</returns>
    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на получение транзакции по ID. TransactionId: {transactionId}.");
        var result = await transactionsService.GetTransactionById(transactionId, cancellationToken);
        logger.LogInformation($"Запрос на получение транзакции по ID завершен. TransactionId: {transactionId}.");
        return Ok(result);
    }

    /// <summary>
    /// Обновляет существующую транзакцию.
    /// </summary>
    /// <param name="request">Данные для обновления транзакции. Содержит UserId, CategoryId nullable, Name nullable, Amount nullable и Date nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на обновление транзакции. TransactionId: {request.TransactionId}.");


        var validationResult = await new UpdateTransactionRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при обновлении транзакции. TransactionId: {request.TransactionId}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        await transactionsService.UpdateTransaction(request.TransactionId, request.CategoryId, request.Name,
            request.Amount, request.Date,
            cancellationToken);
        logger.LogInformation($"Запрос на обновление транзакции завершен. TransactionId: {request.TransactionId}.");
        return Ok();
    }

    /// <summary>
    /// Удаляет транзакцию по идентификатору.
    /// </summary>
    /// <param name="transactionId">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на удаление транзакции. TransactionId: {transactionId}.");
        await transactionsService.DeleteTransaction(transactionId, cancellationToken);
        logger.LogInformation($"Запрос на удаление транзакции завершен. TransactionId: {transactionId}.");
        return Ok();
    }
}