using MapifyBackend.Utility.Security;

namespace MapifyBackend.database_files;

public class AdminApiKeyService
{
    public const string HeaderName = "X-Admin-Api-Key";

    private readonly DatabaseService _db;

    public AdminApiKeyService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<bool> IsValidAdminKey(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        apiKey = apiKey.Trim();
        if (!AdminApiKeyHasher.HasExpectedFormat(apiKey))
            return false;

        string keyHash = AdminApiKeyHasher.Hash(apiKey);
        return await _db.IsActiveAdminApiKeyHash(keyHash);
    }
}
