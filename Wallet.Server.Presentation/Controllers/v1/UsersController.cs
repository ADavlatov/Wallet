using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models;
using Wallet.Server.Application.Models.Users;
using Wallet.Server.Application.Validators;
using Wallet.Server.Domain.Interfaces;
using Wallet.Server.Domain.Interfaces.Services;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

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

    [Authorize]
    [HttpPost("GetUserByUsername")]
    public async Task<IActionResult> GetUserByUsername([FromBody] GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserByUsername(request.Username, cancellationToken));
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserById(userId, cancellationToken));
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        await usersService.UpdateUser(request.UserId, request.Username, request.Password, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await usersService.DeleteUser(userId, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpPost("GetApiKey")]
    public async Task<IActionResult> GetApiKey([FromBody] GetApiKeyRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetApiKey(request.UserId, cancellationToken));
    }

    [Authorize]
    [HttpPut("UpdateApiKey")]
    public async Task<IActionResult> UpdateApiKey([FromBody] UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        await usersService.UpdateApiKey(request.UserId, cancellationToken);
        return Ok();
    }
}