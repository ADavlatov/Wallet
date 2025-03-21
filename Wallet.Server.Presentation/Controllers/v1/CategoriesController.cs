using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Categories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("/api/v1/categories")]
public class CategoriesController(ICategoriesService categoriesService) : ControllerBase
{
    /// <summary>
    /// Добавление новой категории
    /// </summary>
    /// <param name="request">Данные для добавления категории. Содержит UserId, Name и TransactionType</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await categoriesService.AddCategory(request.UserId, request.Name, request.Type, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Получение категорий по типу
    /// </summary>
    /// <param name="request">Данные для фильтрации категорий по типу. Содержит UserId и TransactionType</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список категорий</returns>
    [HttpPost("GetCategoriesByType")]
    public async Task<IActionResult> GetCategoriesByType([FromBody] GetCategoriesByTypeRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoriesByType(request.UserId, request.Type, cancellationToken));
    }

    /// <summary>
    /// Получение категорий по пользователю
    /// </summary>
    /// <param name="request">Данные для фильтрации категорий по пользователю. Содержит UserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список категорий</returns>
    [HttpPost("GetCategoriesByUser")]
    public async Task<IActionResult> GetCategoriesByUser([FromBody] GetCategoriesByUserRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoriesByUser(request.UserId, cancellationToken));
    }

    /// <summary>
    /// Получение категории по названию
    /// </summary>
    /// <param name="request">Данные для поиска категории по названию. Содержит UserId и Name</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Категория</returns>
    [HttpPost("GetCategoryByName")]
    public async Task<IActionResult> GetCategoryByName([FromBody] GetCategoryByNameRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryByName(request.UserId, request.Name, cancellationToken));
    }

    /// <summary>
    /// Получение категории по идентификатору
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Категория</returns>
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        return Ok(await categoriesService.GetCategoryById(categoryId, cancellationToken));
    }

    /// <summary>
    /// Обновление категории
    /// </summary>
    /// <param name="request">Данные для обновления категории. Содержит CategoryId и Name nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await categoriesService.UpdateCategory(request.CategoryId, request.Name, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Удаление категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        await categoriesService.DeleteCategory(categoryId, cancellationToken);
        return Ok();
    }
}