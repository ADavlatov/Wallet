using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wallet.Server.Domain.Exceptions;

namespace Wallet.Server.Presentation;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, $"Произошло необработанное исключение: {context.Exception.Message}");

        if (context.Exception is NotFoundException)
        {
            _logger.LogWarning($"Обработка исключения типа NotFoundException: {context.Exception.Message}");
            context.Result = new NotFoundObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is AlreadyExistsException)
        {
            _logger.LogWarning($"Обработка исключения типа AlreadyExistsException: {context.Exception.Message}");
            context.Result = new ConflictObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is AuthenticationException)
        {
            _logger.LogWarning($"Обработка исключения типа AuthenticationException: {context.Exception.Message}");
            context.Result = new UnauthorizedObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            _logger.LogWarning($"Обработка исключения типа UnauthorizedAccessException: {context.Exception.Message}");
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.ExceptionHandled = true;
        }
        else
        {
            _logger.LogError($"Обработка необработанного исключения (BadRequest): {context.Exception.Message}");
            context.Result = new BadRequestObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.ExceptionHandled = true;
        }
    }
}