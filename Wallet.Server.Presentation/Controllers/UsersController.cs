using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Presentation.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(
    IUsersService usersService,
    ICategoriesService categoriesService,
    ITransactionsService transactionsService) : ControllerBase
{
    // [HttpPost]
    // public async Task<IActionResult> AddUser([FromBody] AddUserRequest request, CancellationToken cancellationToken)
    // {
    //     await usersService.AddUser(request.Username, request.Password, cancellationToken);
    //     return Ok();
    // }

    [HttpGet]
    public async Task<IActionResult> GetUserByUsername([FromQuery] string username, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserByUsername(username, cancellationToken));
    }

    [HttpGet("/{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserById(userId, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        await usersService.UpdateUser(request.UserId, request.Username, request.Password, cancellationToken);
        return Ok();
    }

    [HttpDelete("/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await usersService.DeleteUser(userId, cancellationToken);
        return Ok();
    }
}