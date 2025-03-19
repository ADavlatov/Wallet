using System.Security.Cryptography;

namespace Wallet.Server.Infrastructure.Helpers;

public static class ApiKeyGenerator
{
    public static string GenerateApiKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteData = new byte[32];
        rng.GetBytes(byteData);
        return Convert.ToBase64String(byteData).Replace("+", "").Replace("/", "").Replace("=", "");
    }
}