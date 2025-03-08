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
    [HttpPost("AddGoal")]
    public async Task<IActionResult> AddGoal([FromBody] AddGoalRequest request, CancellationToken cancellationToken)
    {
        await goalsService.AddGoal(request.UserId, request.Name, request.TargetSum, request.Deadline, cancellationToken);
        return Ok();
    }

    [HttpPost("AddSumToGoal")]
    public async Task<IActionResult> AddSumToGoal([FromBody] AddSumToGoalRequest request, CancellationToken cancellationToken)
    {
        await goalsService.AddSumToGoal(request.GoalId, request.Sum, cancellationToken);
        return Ok();
    }

    [HttpPost("GetGoals")]
    public async Task<IActionResult> GetGoalsByUser([FromBody] GetGoalsRequest request, CancellationToken cancellationToken)
    {
        return Ok(await goalsService.GetGoalsByUserId(request.UserId, cancellationToken));
    }

    [HttpGet("{goalId}")]
    public async Task<IActionResult> GetGoal(Guid goalId, CancellationToken cancellationToken)
    {
        return Ok(await goalsService.GetGoalById(goalId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGoal([FromBody] UpdateGoalRequest request, CancellationToken cancellationToken)
    {
        await goalsService.UpdateGoal(request.GoalId, request.Name, request.TargetSum, request.Deadline, cancellationToken);
        return Ok();
    }

    [HttpDelete("{goalId}")]
    public async Task<IActionResult> DeleteGoal(Guid goalId, CancellationToken cancellationToken)
    {
        await goalsService.DeleteGoal(goalId, cancellationToken);
        return Ok();
    }
}