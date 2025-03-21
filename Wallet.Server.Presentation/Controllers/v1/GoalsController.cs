using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Goals;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

/// <summary>
/// Контроллер для работы с целями
/// </summary>
/// <param name="goalsService">Сервис целей</param>
[Authorize]
[ApiController]
[Route("/api/v1/goals")]
public class GoalsController(
    IGoalsService goalsService,
    ILogger<GoalsController> logger,
    IValidator<AddGoalRequest> addGoalValidator,
    IValidator<AddSumToGoalRequest> addSumToGoalValidator,
    IValidator<UpdateGoalRequest> updateGoalValidator) : ControllerBase
{
    /// <summary>
    /// Добавляет новую цель для пользователя.
    /// </summary>
    /// <param name="request">Данные для добавления цели. Содержит UserId, Name, TargetSum, Deadline nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("AddGoal")]
    public async Task<IActionResult> AddGoal([FromBody] AddGoalRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на добавление цели. UserId: {request.UserId}, Name: {request.Name}, TargetSum: {request.TargetSum}, Deadline: {request.Deadline}.");
        var validationResult = await addGoalValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при добавлении цели. UserId: {request.UserId}, Name: {request.Name}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        await goalsService.AddGoal(request.UserId, request.Name, request.TargetSum, request.Deadline,
            cancellationToken);
        logger.LogInformation($"Запрос на добавление цели завершен. UserId: {request.UserId}, Name: {request.Name}.");
        return Ok();
    }

    /// <summary>
    /// Добавляет сумму к существующей цели.
    /// </summary>
    /// <param name="request">Данные для добавления суммы к цели. Содержит GoalId и Sum</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("AddSumToGoal")]
    public async Task<IActionResult> AddSumToGoal([FromBody] AddSumToGoalRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на добавление суммы к цели. GoalId: {request.GoalId}, Sum: {request.Sum}.");
        var validationResult = await addSumToGoalValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при добавлении суммы к цели. GoalId: {request.GoalId}, Sum: {request.Sum}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        await goalsService.AddSumToGoal(request.GoalId, request.Sum, cancellationToken);
        logger.LogInformation(
            $"Запрос на добавление суммы к цели завершен. GoalId: {request.GoalId}, Sum: {request.Sum}.");
        return Ok();
    }

    /// <summary>
    /// Получает список целей для пользователя.
    /// </summary>
    /// <param name="request">Данные для получения цели. Содержит UserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список целей</returns>
    [HttpPost("GetGoals")]
    public async Task<IActionResult> GetGoalsByUser([FromBody] GetGoalsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на получение целей пользователя. UserId: {request.UserId}.");
        var result = await goalsService.GetGoalsByUserId(request.UserId, cancellationToken);
        logger.LogInformation(
            $"Запрос на получение целей пользователя завершен. UserId: {request.UserId}. Количество целей: {result.Count}.");
        return Ok(result);
    }

    /// <summary>
    /// Получает детальную информацию о цели по её идентификатору.
    /// </summary>
    /// <param name="goalId">Идентификатор цели.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Цель</returns>
    [HttpGet("{goalId}")]
    public async Task<IActionResult> GetGoal(Guid goalId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на получение цели по ID. GoalId: {goalId}.");
        var result = await goalsService.GetGoalById(goalId, cancellationToken);
        logger.LogInformation($"Запрос на получение цели по ID завершен. GoalId: {goalId}.");
        return Ok(result);
    }

    /// <summary>
    /// Обновляет информацию о цели.
    /// </summary>
    /// <param name="request">Данные для обновления цели. Содержит GoalId, Name nullable, TargetSum nullable, Deadline nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateGoal([FromBody] UpdateGoalRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            $"Начало запроса на обновление цели. GoalId: {request.GoalId}, Name: {request.Name}, TargetSum: {request.TargetSum}, Deadline: {request.Deadline}.");
        var validationResult = await updateGoalValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при обновлении цели. GoalId: {request.GoalId}, Name: {request.Name}, TargetSum: {request.TargetSum}, Deadline: {request.Deadline}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        await goalsService.UpdateGoal(request.GoalId, request.Name, request.TargetSum, request.Deadline,
            cancellationToken);
        logger.LogInformation($"Запрос на обновление цели завершен. GoalId: {request.GoalId}.");
        return Ok();
    }

    /// <summary>
    /// Удаляет цель по её идентификатору.
    /// </summary>
    /// <param name="goalId">Идентификатор цели.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpDelete("{goalId}")]
    public async Task<IActionResult> DeleteGoal(Guid goalId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на удаление цели. GoalId: {goalId}.");
        await goalsService.DeleteGoal(goalId, cancellationToken);
        logger.LogInformation($"Запрос на удаление цели завершен. GoalId: {goalId}.");
        return Ok();
    }
}