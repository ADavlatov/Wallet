using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Presentation.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController(ICategoriesService categoriesService, ITransactionsService transactionsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.Type, true, out TransactionTypes type))
        {
            throw new RequestValidateException();
        }

        await categoriesService.AddCategory(request.UserId, request.Name, type, cancellationToken);

        return Ok();
    }

    [HttpPost("/byType")]
    public async Task<IActionResult> GetCategoriesByType([FromBody] GetCategoriesByTypeRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.Type, true, out TransactionTypes type))
        {
            throw new RequestValidateException();
        }

        return Ok(await categoriesService.GetCategoriesByType(request.UserId, type, cancellationToken));
    }

    [HttpPost("/byName")]
    public async Task<IActionResult> GetCategoryByName([FromBody] GetCategoryByNameRequest request, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryByName(request.UserId, request.Name, cancellationToken));
    }

    [HttpGet("/{userId}")]
    public async Task<IActionResult> GetCategoriesByUser(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoriesByUser(userId, cancellationToken));
    }
    
    [HttpGet("/{categoryId}/transactions")]
    public async Task<IActionResult> GetTransactionsByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return Ok(await transactionsService.GetTransactionsByCategory(categoryId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var isValid = Enum.TryParse(request.Type, true, out TransactionTypes type);
        if (request.Type != null && !isValid)
        {
            throw new RequestValidateException();
        }
        
        await categoriesService.UpdateCategory(request.CategoryId, request.Name, type, cancellationToken);
        
        return Ok();
    }

    [HttpDelete("/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        await categoriesService.DeleteCategory(id, cancellationToken);
        return Ok();
    }
}