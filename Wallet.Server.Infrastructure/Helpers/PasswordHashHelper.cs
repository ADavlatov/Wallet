using System.Security.Cryptography;

namespace Wallet.Server.Infrastructure.Helpers;

public static class PasswordHashHelper
{
    private const int IterationCount = 10000;
    private const int SaltLength = 32;

    public static bool ValidateHash(string? password, byte[]? salt, byte[] expectedHash)
    {
        if (password == null) return false;

        var (hash, _) = HashPassword(password, salt);
        return hash.SequenceEqual(expectedHash);
    }

    public static (byte[] Hash, byte[] Salt) HashPassword(string password, byte[]? salt = null)
    {
        Rfc2898DeriveBytes rfc2898DeriveBytes;
        if (salt != null)
        {
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA1);
        }
        else
        {
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltLength, IterationCount, HashAlgorithmName.SHA1);
        }

        var hashBytes = rfc2898DeriveBytes.GetBytes(20);
        var saltBytes = salt ?? rfc2898DeriveBytes.Salt;

        return (hashBytes, saltBytes);
    }
}