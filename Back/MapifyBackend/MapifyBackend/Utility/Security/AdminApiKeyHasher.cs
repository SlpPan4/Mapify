using System.Security.Cryptography;
using System.Text;

namespace MapifyBackend.Utility.Security;

public static class AdminApiKeyHasher
{
    public const int ExpectedKeyLength = 24;

    public static string Hash(string apiKey)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool HasExpectedFormat(string apiKey)
    {
        return apiKey.Length == ExpectedKeyLength && apiKey.All(IsAllowedKeyCharacter);
    }

    private static bool IsAllowedKeyCharacter(char value)
    {
        return char.IsAsciiLetterOrDigit(value) || value is '_' or '-';
    }
}
