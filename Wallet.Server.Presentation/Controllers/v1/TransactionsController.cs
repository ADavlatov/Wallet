using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Transactions;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("/api/v1/transactions")]
public class TransactionsController(ITransactionsService transactionsService) : ControllerBase
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
        await transactionsService.AddTransaction(request.UserId, request.CategoryId, request.Name, request.Amount,
            request.Date, request.Type, cancellationToken);

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
        return Ok(await transactionsService.GetTransactionsByType(request.UserId, request.Type, cancellationToken));
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
        return Ok(await transactionsService.GetTransactionsByCategory(request.CategoryId, cancellationToken));
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
        return Ok(await transactionsService.GetTransactionById(transactionId, cancellationToken));
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
        await transactionsService.UpdateTransaction(request.TransactionId, request.CategoryId, request.Name,
            request.Amount, request.Date,
            cancellationToken);

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
        await transactionsService.DeleteTransaction(transactionId, cancellationToken);
        return Ok();
    }
}