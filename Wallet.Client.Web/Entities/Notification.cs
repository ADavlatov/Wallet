using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Entities;

public class Notification
{
    public string Id { get; set; }
    public string UserId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    
    public DateOnly DateOnly { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public TimeOnly TimeOnly { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
}