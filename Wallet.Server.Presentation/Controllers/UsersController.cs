using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Presentation.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    // [HttpPost]
    // public async Task<IActionResult> AddUser([FromBody] AddUserRequest request, CancellationToken cancellationToken)
    // {
    //     await usersService.AddUser(request.Username, request.Password, cancellationToken);
    //     return Ok();
    // }

    [HttpPost]
    public async Task<IActionResult> GetUserByUsername([FromBody] GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserByUsername(request.Username, cancellationToken));
    }

    [HttpGet("/{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserById(id, cancellationToken));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
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