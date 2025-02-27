using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Application.Models.Users;
using Wallet.Server.Application.Validators;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var validation = await new AuthValidator().ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }
        
        return Ok(await usersService.SignUp(request.Username, request.Password, cancellationToken));
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] AuthRequest request, CancellationToken cancellationToken)
    {
        var validation = await new AuthValidator().ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }
        
        return Ok(await usersService.SignIn(request.Username, request.Password, cancellationToken));
    }

    [HttpPost("RefreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.RefreshTokens(request.RefreshToken, cancellationToken));
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