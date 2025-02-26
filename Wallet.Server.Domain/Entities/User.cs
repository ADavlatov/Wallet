namespace Wallet.Server.Domain.Entities;

public class User(string username, string password, string passwordSalt)
{
    public Guid Id { get; init; }
    public long? TelegramUserId { get; set; }
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string PasswordSalt { get; set; } = passwordSalt;
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
}