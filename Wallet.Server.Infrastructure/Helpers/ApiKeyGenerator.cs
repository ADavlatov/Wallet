using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Wallet.Server.Infrastructure.Helpers;

public static class ApiKeyGenerator
{
    public static string GenerateApiKey(ILogger logger)
    {
        logger.LogInformation("Запрос на генерацию API ключа.");
        using var rng = RandomNumberGenerator.Create();
        var byteData = new byte[32];
        rng.GetBytes(byteData);
        var apiKey = Convert.ToBase64String(byteData).Replace("+", "").Replace("/", "").Replace("=", "");
        logger.LogInformation($"API ключ успешно сгенерирован: {apiKey}");
        return apiKey;
    }
}