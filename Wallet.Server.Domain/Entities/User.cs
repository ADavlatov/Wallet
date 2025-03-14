namespace Wallet.Server.Domain.Entities;

public class User(string username, byte[] passwordHash, byte[] passwordSalt, string apiKey)
{
    public Guid Id { get; init; }
    public long? TelegramUserId { get; set; }
    public string? Email { get; set; }
    public string Username { get; set; } = username;
    public byte[] PasswordHash { get; set; } = passwordHash;
    public byte[] PasswordSalt { get; set; } = passwordSalt;
    public string ApiKey { get; set; } = apiKey;
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
}