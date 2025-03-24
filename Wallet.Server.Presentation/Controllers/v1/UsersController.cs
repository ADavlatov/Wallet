using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Server.Application.Models.Users;
using Wallet.Server.Application.Validators.Users;
using Wallet.Server.Domain.Interfaces.Services;

namespace Wallet.Server.Presentation.Controllers.v1;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
/// <param name="usersService">Сервис пользователей</param>
[ApiController]
[Route("/api/v1/users")]
public class UsersController(
    IUsersService usersService,
    ILogger<UsersController> logger) : ControllerBase
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
        logger.LogInformation($"Начало запроса на регистрацию. Username: {request.Username}.");
        var validationResult = await new AuthRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при регистрации. Username: {request.Username}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var result = await usersService.SignUp(request.Username, request.Password, cancellationToken);
        logger.LogInformation(
            $"Запрос на регистрацию завершен. Username: {request.Username}, UserId: {result.UserId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на авторизацию. Username: {request.Username}.");
        var validationResult = await new AuthRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при авторизации. Username: {request.Username}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var result = await usersService.SignIn(request.Username, request.Password, cancellationToken);
        logger.LogInformation(
            $"Запрос на авторизацию завершен. Username: {request.Username}, UserId: {result.UserId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на обновление токенов.");
        var result = await usersService.RefreshTokens(request.RefreshToken, cancellationToken);
        logger.LogInformation($"Запрос на обновление токенов завершен. UserId: {result.UserId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на получение пользователя по имени. Username: {request.Username}.");
        var validationResult = await new GetUserByUsernameRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при получении пользователя по имени. Username: {request.Username}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }

        var result = await usersService.GetUserByUsername(request.Username, cancellationToken);
        logger.LogInformation(
            $"Запрос на получение пользователя по имени завершен. Username: {request.Username}, UserId: {result.Id}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на получение пользователя по ID. UserId: {userId}.");
        var result = await usersService.GetUserById(userId, cancellationToken);
        logger.LogInformation($"Запрос на получение пользователя по ID завершен. UserId: {userId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на обновление пользователя. UserId: {request.UserId}.");
        await usersService.UpdateUser(request.UserId, request.Username, request.Password, cancellationToken);
        logger.LogInformation($"Запрос на обновление пользователя завершен. UserId: {request.UserId}.");
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
        logger.LogInformation($"Начало запроса на удаление пользователя. UserId: {userId}.");
        await usersService.DeleteUser(userId, cancellationToken);
        logger.LogInformation($"Запрос на удаление пользователя завершен. UserId: {userId}.");
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
        logger.LogInformation($"Начало запроса на получение API ключа. UserId: {request.UserId}.");
        var result = await usersService.GetApiKey(request.UserId, cancellationToken);
        logger.LogInformation($"Запрос на получение API ключа завершен. UserId: {request.UserId}.");
        return Ok(result);
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
        logger.LogInformation($"Начало запроса на обновление API ключа. UserId: {request.UserId}.");
        await usersService.UpdateApiKey(request.UserId, cancellationToken);
        logger.LogInformation($"Запрос на обновление API ключа завершен. UserId: {request.UserId}.");
        return Ok();
    }
    
    /// <summary>
    /// Валидация API ключа
    /// </summary>
    /// <param name="request">Данные для валидации ключа. Содержит ApiKey, TelegramUserId</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус операции</returns>
    [HttpPost("ValidateApiKey")]
    public async Task<IActionResult> ValidateApiKey([FromBody] ValidateApiKeyRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Начало запроса на валидацию API ключа. ApiKey: {request.ApiKey}, TelegramUserId: {request.TelegramUserId}");
        var validationResult = await new ValidateApiKeyRequestValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                $"Ошибка валидации при валидации API ключа. ApiKey: {request.ApiKey}. Ошибки: {string.Join(", ", validationResult.Errors)}");
            return BadRequest(validationResult.Errors);
        }
        await usersService.ValidateApiKey(request.ApiKey, request.TelegramUserId, cancellationToken);
        logger.LogInformation($"Запроса на валидацию API ключа завершен. ApiKey: {request.ApiKey}, TelegramUserId: {request.TelegramUserId}");
        return Ok();
    }
}