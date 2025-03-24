using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Users;

public class RefreshTokenRequest(string refreshToken)
{
    [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; }
}