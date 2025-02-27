namespace Wallet.Server.Domain.Entities;

public class User(string username, byte[] password, byte[] passwordSalt)
{
    public Guid Id { get; init; }
    public long? TelegramUserId { get; set; }
    public string Username { get; set; } = username;
    public byte[] PasswordHash { get; set; } = password;
    public byte[] PasswordSalt { get; set; } = passwordSalt;
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
}