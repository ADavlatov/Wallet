namespace Wallet.Server.Domain.Entities;

public class Notification(string name, string description, DateTime dateTime)
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public DateTime DateTime { get; set; } = dateTime;
}