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

    [HttpGet]
    public async Task<IActionResult> GetCategoriesByType([FromQuery] Guid userId, string type, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(type, true, out TransactionTypes transactionType))
        {
            throw new RequestValidateException();
        }

        return Ok(await categoriesService.GetCategoriesByType(userId, transactionType, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoryByName([FromQuery] Guid userId, string name, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryByName(userId, name, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesByUser([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoriesByUser(userId, cancellationToken));
    }

    [HttpGet("/{categoryId}")]
    public async Task<IActionResult> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryById(categoryId, cancellationToken));
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
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        await categoriesService.DeleteCategory(categoryId, cancellationToken);
        return Ok();
    }
}