namespace Wallet.Server.Domain.Exceptions;

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException() : base("Entity already exists")
    {
    }
    
    public AlreadyExistsException(string message) : base(message)
    {
    }
}