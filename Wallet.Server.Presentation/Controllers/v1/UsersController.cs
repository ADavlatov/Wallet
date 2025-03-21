using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Users;
using Wallet.Server.Application.Validators;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

[ApiController]
[Route("/api/v1/users")]
public class UsersController(IUsersService usersService) : ControllerBase
{
    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации. Содержит Username и Password</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>AccessToken, RefreshToken и UserId</returns>
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

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="request">Данные для аутентификации. Содержит Username и Password</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>AccessToken, RefreshToken и UserId</returns>
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

    /// <summary>
    /// Обновление токенов сессии
    /// </summary>
    /// <param name="request">Данные для обновления пары токенов. Содержит RefreshToken</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>AccessToken, RefreshToken и UserId</returns>
    [HttpPost("RefreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await usersService.RefreshTokens(request.RefreshToken, cancellationToken));
    }

    /// <summary>
    /// Получение информации о пользователе по имени пользователя
    /// </summary>
    /// <param name="request">Данные для получения пользователя. Содержит Username</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные пользователя в случае успеха</returns>
    [Authorize]
    [HttpPost("GetUserByUsername")]
    public async Task<IActionResult> GetUserByUsername([FromBody] GetUserByUsernameRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserByUsername(request.Username, cancellationToken));
    }

    /// <summary>
    /// Получение информации о пользователе по идентификатору
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные пользователя в случае успеха</returns>
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetUserById(userId, cancellationToken));
    }

    /// <summary>
    /// Обновление данных пользователя
    /// </summary>
    /// <param name="request">Данные для обновления пользователя. Содержит UserId, Username nullable и Password nullable</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        await usersService.UpdateUser(request.UserId, request.Username, request.Password, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Удаление пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [Authorize]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await usersService.DeleteUser(userId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Получение API ключа пользователя
    /// </summary>
    /// <param name="request">Данные для получения api ключа. Содержит UserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>API ключ пользователя в случае успеха</returns>
    [Authorize]
    [HttpPost("GetApiKey")]
    public async Task<IActionResult> GetApiKey([FromBody] GetApiKeyRequest request, CancellationToken cancellationToken)
    {
        return Ok(await usersService.GetApiKey(request.UserId, cancellationToken));
    }

    /// <summary>
    /// Обновление API ключа пользователя
    /// </summary>
    /// <param name="request">Данные для обновления api ключа. Содержит UserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [Authorize]
    [HttpPut("UpdateApiKey")]
    public async Task<IActionResult> UpdateApiKey([FromBody] UpdateApiKeyRequest request,
        CancellationToken cancellationToken)
    {
        await usersService.UpdateApiKey(request.UserId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Валидация API ключа для привязки telegram
    /// </summary>
    /// <param name="request">Данные для валидации api ключа. Содержит ApiKey и TelegramUserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("ValidateApiKey")]
    public async Task<IActionResult> ValidateApiKey([FromBody] ValidateApiKeyRequest request,
        CancellationToken cancellationToken)
    {
        await usersService.ValidateApiKey(request.ApiKey, request.TelegramUserId, cancellationToken);
        return Ok();
    }
}