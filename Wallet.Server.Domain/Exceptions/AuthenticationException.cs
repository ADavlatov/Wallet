namespace Wallet.Server.Domain.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException() : base("Invalid data")
    {
    }
    
    public AuthenticationException(string message) : base(message)
    {
    }
}