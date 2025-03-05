using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class AuthRequest
{
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
};