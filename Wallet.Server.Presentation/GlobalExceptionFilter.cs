using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wallet.Server.Domain.Exceptions;

namespace Wallet.Server.Presentation;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "Произошло необработанное исключение: " +
                                            "{Message}", context.Exception.Message);

        if (context.Exception is NotFoundException)
        {
            logger.LogWarning("Обработка исключения типа NotFoundException: " +
                               "{Message}", context.Exception.Message);
            
            context.Result = new NotFoundObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        else if (context.Exception is AlreadyExistsException)
        {
            logger.LogWarning("Обработка исключения типа AlreadyExistsException: " +
                               "{Message}", context.Exception.Message);
            
            context.Result = new ConflictObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
        }
        else if (context.Exception is AuthenticationException)
        {
            logger.LogWarning("Обработка исключения типа AuthenticationException: " +
                               "{Message}", context.Exception.Message);
            
            context.Result = new UnauthorizedObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            logger.LogWarning("Обработка исключения типа UnauthorizedAccessException: " +
                               "{Message}", context.Exception.Message);
            
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        else
        {
            logger.LogError("Обработка необработанного исключения (BadRequest): " +
                             "{Message}", context.Exception.Message);
            
            context.Result = new BadRequestObjectResult(new { context.Exception.Message });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        context.ExceptionHandled = true;
    }
}