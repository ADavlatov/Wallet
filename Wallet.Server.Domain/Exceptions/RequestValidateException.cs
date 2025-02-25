namespace Wallet.Server.Domain.Exceptions;

public class RequestValidateException : Exception
{
    public RequestValidateException() : base("Request validation error")
    {
    }

    public RequestValidateException(string message) : base(message)
    {
    }
}