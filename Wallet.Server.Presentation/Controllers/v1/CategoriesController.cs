using Wallet.Server.Application.Models.Categories;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Контроллер для работы с категориями
/// </summary>
/// <param name="categoriesService">Сервис категорий</param>
[Authorize]
[ApiController]
[Route("/api/v1/categories")]
public class CategoriesController(ICategoriesService categoriesService, ILogger<CategoriesController> logger) : ControllerBase
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
        logger.LogInformation($"Начало запроса на добавление категории. Пользователь: {request.UserId}, название: {request.Name}, тип: {request.Type}.");
        await categoriesService.AddCategory(request.UserId, request.Name, request.Type, cancellationToken);
        logger.LogInformation($"Запрос на добавление категории завершен. Пользователь: {request.UserId}, название: {request.Name}.");
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
        logger.LogInformation($"Начало запроса на получение категорий по типу. Пользователь: {request.UserId}, тип: {request.Type}.");
        var result = await categoriesService.GetCategoriesByType(request.UserId, request.Type, cancellationToken);
        logger.LogInformation($"Запрос на получение категорий по типу завершен. Пользователь: {request.UserId}, тип: {request.Type}. Получено категорий: {result.Count}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на получение категорий для пользователя. Пользователь: {request.UserId}.");
        var result = await categoriesService.GetCategoriesByUser(request.UserId, cancellationToken);
        logger.LogInformation($"Запрос на получение категорий для пользователя завершен. Пользователь: {request.UserId}. Получено категорий: {result.Count}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на получение категории по названию. Пользователь: {request.UserId}, название: {request.Name}.");
        var result = await categoriesService.GetCategoryByName(request.UserId, request.Name, cancellationToken);
        logger.LogInformation($"Запрос на получение категории по названию завершен. Пользователь: {request.UserId}, название: {request.Name}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на получение категории по идентификатору. ID: {categoryId}.");
        var result = await categoriesService.GetCategoryById(categoryId, cancellationToken);
        logger.LogInformation($"Запрос на получение категории по идентификатору завершен. ID: {categoryId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на обновление категории. ID: {request.CategoryId}, новое название: {request.Name}.");
        await categoriesService.UpdateCategory(request.CategoryId, request.Name, cancellationToken);
        logger.LogInformation($"Запрос на обновление категории завершен. ID: {request.CategoryId}, новое название: {request.Name}.");
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
        logger.LogInformation($"Начало запроса на удаление категории. ID: {categoryId}.");
        await categoriesService.DeleteCategory(categoryId, cancellationToken);
        logger.LogInformation($"Запрос на удаление категории завершен. ID: {categoryId}.");
        return Ok();
    }
}