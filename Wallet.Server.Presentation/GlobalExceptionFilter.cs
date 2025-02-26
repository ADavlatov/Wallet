using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wallet.Server.Domain.Exceptions;

namespace Wallet.Server.Presentation;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Console.WriteLine(context.Exception);
        if (context.Exception is NotFoundException)
        {
            context.Result = new NotFoundResult();
            context.ExceptionHandled = true;
        }
        if (context.Exception is AlreadyExistsException)
        {
            context.Result = new ContentResult();
            context.ExceptionHandled = true;
        }
        if (context.Exception is AuthenticationException)
        {
            context.Result = new UnauthorizedResult();
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new BadRequestResult();
            context.ExceptionHandled = true;
        }
    }
}