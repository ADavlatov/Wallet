using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Categories;
using Wallet.Server.Domain.Enums;
using Wallet.Server.Domain.Exceptions;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/categories")]
public class CategoriesController(ICategoriesService categoriesService) : ControllerBase
{
    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request, CancellationToken cancellationToken)
    {
        await categoriesService.AddCategory(request.UserId, request.Name, request.Type, cancellationToken);
        return Ok();
    }

    [HttpPost("GetCategoriesByType")]
    public async Task<IActionResult> GetCategoriesByType([FromBody] GetCategoriesByTypeRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.Type, true, out TransactionTypes transactionType))
        {
            throw new RequestValidateException();
        }

        return Ok(await categoriesService.GetCategoriesByType(request.UserId, transactionType, cancellationToken));
    }

    [HttpPost("GetCategoriesByUser")]
    public async Task<IActionResult> GetCategoriesByUser([FromBody] GetCategoriesByUserRequest request, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoriesByUser(request.UserId, cancellationToken));
    }

    [HttpPost("GetCategoryByName")]
    public async Task<IActionResult> GetCategoryByName([FromBody] GetCategoryByNameRequest request, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryByName(request.UserId, request.Name, cancellationToken));
    }

    [HttpGet("{categoryId}")]
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

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        await categoriesService.DeleteCategory(categoryId, cancellationToken);
        return Ok();
    }
}