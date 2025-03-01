using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Application.Models.Transactions;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
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
        if (!Enum.TryParse(request.Type, true, out TransactionTypes type))
        {
            throw new RequestValidateException();
        }

        await transactionsService.AddTransaction(request.UserId, request.CategoryId, request.Name, request.Amount,
            request.Date, type, cancellationToken);

        return Ok();
    }

    [HttpPost("GetTransactionsByType")]
    public async Task<IActionResult> GetTransactionsByType([FromBody] GetTransactionsByTypeRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.Type, true, out TransactionTypes transactionType))
        {
            throw new RequestValidateException();
        }
        return Ok(await transactionsService.GetTransactionsByType(request.UserId, transactionType, cancellationToken));
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
        var isValid = Enum.TryParse(request.Type, true, out TransactionTypes type);
        if (request.Type != null && !isValid)
        {
            throw new RequestValidateException();
        }

        await transactionsService.UpdateTransaction(request.TransactionId, request.Name, request.Amount, request.Date, type, cancellationToken);

        return Ok();
    }

    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken)
    {
        await transactionsService.DeleteTransaction(transactionId, cancellationToken);
        return Ok();
    }
}