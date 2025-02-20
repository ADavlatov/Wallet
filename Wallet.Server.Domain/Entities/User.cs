namespace Wallet.Server.Domain.Entities;

public class User(string username, string password)
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}