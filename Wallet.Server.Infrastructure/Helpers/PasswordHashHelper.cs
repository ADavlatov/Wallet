using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Wallet.Server.Domain.DTOs;

namespace Wallet.Server.Infrastructure.Helpers;

public static class PasswordHashHelper
{
    private const int IterationCount = 10000;
    private const int SaltLength = 32;

    public static bool ValidateHash(string? password, byte[]? salt, byte[] expectedHash, ILogger logger)
    {
        logger.LogInformation("Запрос на валидацию пароля.");
        if (password == null)
        {
            logger.LogInformation("Пароль для валидации пуст.");
            return false;
        }

        var (hash, _) = HashPassword(password, logger, salt);
        return hash.SequenceEqual(expectedHash);
    }

    public static PasswordDto HashPassword(string password, ILogger logger, byte[]? salt = null)
    {
        logger.LogInformation("Запрос на хеширование пароля.");
        Rfc2898DeriveBytes rfc2898DeriveBytes;
        if (salt != null)
        {
            logger.LogInformation("Используется предоставленная соль для хеширования.");
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA1);
        }
        else
        {
            logger.LogInformation("Генерация новой соли для хеширования.");
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltLength, IterationCount, HashAlgorithmName.SHA1);
        }

        logger.LogInformation("Хеширование пароля завершено.");
        return new PasswordDto(rfc2898DeriveBytes.GetBytes(20), salt ?? rfc2898DeriveBytes.Salt);
    }
}