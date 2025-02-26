using System.Security.Cryptography;

namespace Wallet.Server.Infrastructure.Helpers;

public class PasswordHashHelper
{

    private const int IterationCount = 10000;
    private const int SaltLength = 32;

    public static bool ValidateHash(string? password, string? salt, string expectedHash)
    {
        if (password == null) return false;

        var (hash, _) = HashPassword(password, salt);
        return string.Equals(expectedHash, hash, StringComparison.Ordinal);
    }

    public static (string Hash, string Salt) HashPassword(string password, string? salt = null)
    {
        Rfc2898DeriveBytes rfc2898DeriveBytes;
        if (salt != null)
        {
            var saltDecoded = Convert.FromBase64String(salt);
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltDecoded, IterationCount, HashAlgorithmName.SHA1);
        }
        else
        {
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltLength, IterationCount, HashAlgorithmName.SHA1);
        }

        var hashBytes = rfc2898DeriveBytes.GetBytes(20);
        var saltBytes = rfc2898DeriveBytes.Salt;

        var hashResult = Convert.ToBase64String(hashBytes);
        var saltResult = salt ?? Convert.ToBase64String(saltBytes);
        return (hashResult, saltResult);
    }
}