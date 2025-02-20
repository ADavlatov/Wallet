namespace Wallet.Server.Domain.Entities;

public class User(string username, string password)
{
    public Guid UserId { get; init; }
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
}