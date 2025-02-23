namespace Wallet.Server.Domain.Entities;

public class User(string username, string password, string salt)
{
    public Guid Id { get; init; }
    public long? TelegramUserId { get; set; }
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Salt { get; set; } = salt;
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
}