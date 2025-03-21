using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Goals;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[Authorize]
[ApiController]
[Route("/api/v1/goals")]
public class GoalsController(IGoalsService goalsService) : ControllerBase
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
        await goalsService.AddGoal(request.UserId, request.Name, request.TargetSum, request.Deadline,
            cancellationToken);
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
        await goalsService.AddSumToGoal(request.GoalId, request.Sum, cancellationToken);
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
        return Ok(await goalsService.GetGoalsByUserId(request.UserId, cancellationToken));
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
        return Ok(await goalsService.GetGoalById(goalId, cancellationToken));
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
        await goalsService.UpdateGoal(request.GoalId, request.Name, request.TargetSum, request.Deadline,
            cancellationToken);
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
        await goalsService.DeleteGoal(goalId, cancellationToken);
        return Ok();
    }
}