using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Domain.Interfaces;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var result = await usersService.SignIn(request.Username, request.Password, cancellationToken);
        return Ok(new TokensResponse(result.Item1, result.Item2));
    }

    [HttpPost("LogIn")]
    public async Task<IActionResult> LogIn([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var result = await usersService.LogIn(request.Username, request.Password, cancellationToken);
        return Ok(new TokensResponse(result.Item1, result.Item2));
    }

    [HttpPost("RefreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensRequest request, CancellationToken cancellationToken)
    {
        var result = await usersService.RefreshTokens(request.RefreshToken, cancellationToken);
        return Ok(new TokensResponse(result.Item1, result.Item2));
    }

    [HttpPost("GetUserByUsername")]
    public async Task<IActionResult> GetUserByUsername([FromBody] GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserByUsername(request.Username, cancellationToken));
    }

    [HttpGet("{userId}")]
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

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await usersService.DeleteUser(userId, cancellationToken);
        return Ok();
    }
}