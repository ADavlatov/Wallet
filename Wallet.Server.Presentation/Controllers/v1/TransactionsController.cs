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
    [HttpPost("AddTransaction")]
    public async Task<IActionResult> AddTransaction([FromBody] AddTransactionRequest request, CancellationToken cancellationToken)
    {
        await transactionsService.AddTransaction(request.UserId, request.CategoryId, request.Name, request.Amount,
            request.Date, request.Type, cancellationToken);

        return Ok();
    }

    [HttpPost("GetTransactionsByType")]
    public async Task<IActionResult> GetTransactionsByType([FromBody] GetTransactionsByTypeRequest request, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionsByType(request.UserId, request.Type, cancellationToken));
    }

    [HttpPost("GetTransactionsByCategory")]
    public async Task<IActionResult> GetTransactionsByCategory([FromBody] GetTransactionsByCategoryRequest request, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionsByCategory(request.CategoryId, cancellationToken));
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionById(transactionId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        await transactionsService.UpdateTransaction(request.TransactionId, request.CategoryId, request.Name, request.Amount, request.Date, cancellationToken);

        return Ok();
    }

    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        await transactionsService.DeleteTransaction(transactionId, cancellationToken);
        return Ok();
    }
}