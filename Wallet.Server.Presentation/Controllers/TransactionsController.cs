using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Presentation.Controllers;

[ApiController]
[Route("api/v1/transactions")]
public class TransactionsController(ITransactionsService transactionsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddTransaction([FromBody] AddTransactionRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.Type, true, out TransactionTypes type))
        {
            throw new RequestValidateException();
        }

        await transactionsService.AddTransaction(request.UserId, request.CategoryId, request.Name, request.Amount,
            request.Date, type, cancellationToken);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionsByType([FromQuery] Guid userId, string type, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(type, true, out TransactionTypes transactionType))
        {
            throw new RequestValidateException();
        }
        return Ok(await transactionsService.GetTransactionsByType(userId, transactionType, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionsByCategory([FromQuery] Guid categoryId, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionsByCategory(categoryId, cancellationToken));
    }

    [HttpGet("/{transactionId}")]
    public async Task<IActionResult> GetTransactionById(Guid transactionId, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionById(transactionId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var isValid = Enum.TryParse(request.Type, true, out TransactionTypes type);
        if (request.Type != null && !isValid)
        {
            throw new RequestValidateException();
        }

        await transactionsService.UpdateTransaction(request.TransactionId, request.Name, request.Amount, request.Date, type, cancellationToken);

        return Ok();
    }

    [HttpDelete("/{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        await transactionsService.RemoveTransaction(transactionId, cancellationToken);
        return Ok();
    }
}