using System.Security.Cryptography;
using System.Text;

namespace MapifyBackend.Utility.Api;

/// <summary>
/// Requires the <c>X-Api-Key</c> header on admin and mutating routes.
/// Protects everything under <c>/api/submissions/admin</c> and <c>/api/admin/maps</c>,
/// and all POST/PUT/PATCH/DELETE requests to <c>/api/strats</c>, <c>/api/categories</c>,
/// and <c>/api/operators</c>. Public reads, public submissions, and CORS
/// preflight (OPTIONS) requests pass through without a key.
/// </summary>
public class ApiKeyAuthMiddleware
{
    public const string HeaderName = "X-Api-Key";

    private static readonly string[] AlwaysProtectedPrefixes =
    [
        "/api/submissions/admin",
        "/api/admin/maps"
    ];

    private static readonly string[] MutatingProtectedPrefixes =
    [
        "/api/strats",
        "/api/categories",
        "/api/operators"
    ];

    private readonly RequestDelegate _next;
    private readonly byte[] _adminKeyBytes;

    public ApiKeyAuthMiddleware(RequestDelegate next, string adminKey)
    {
        _next = next;
        _adminKeyBytes = Encoding.UTF8.GetBytes(adminKey);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!RequiresApiKey(context.Request) || HasValidKey(context.Request))
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(
            ApiResponse.Failure(StatusCodes.Status401Unauthorized, "Missing or invalid API key"));
    }

    private static bool RequiresApiKey(HttpRequest request)
    {
        // CORS preflight must pass through without a key.
        if (HttpMethods.IsOptions(request.Method))
            return false;

        string path = request.Path.Value ?? string.Empty;

        // Admin endpoints (submission review, map/bombsite management) are always protected.
        if (AlwaysProtectedPrefixes.Any(prefix => PathStartsWithSegment(path, prefix)))
            return true;

        // Mutating requests to the main resources are protected.
        bool isMutating = HttpMethods.IsPost(request.Method)
                          || HttpMethods.IsPut(request.Method)
                          || HttpMethods.IsPatch(request.Method)
                          || HttpMethods.IsDelete(request.Method);

        return isMutating && MutatingProtectedPrefixes.Any(prefix => PathStartsWithSegment(path, prefix));
    }

    private bool HasValidKey(HttpRequest request)
    {
        string? providedKey = request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrEmpty(providedKey))
            return false;

        byte[] providedKeyBytes = Encoding.UTF8.GetBytes(providedKey);
        return providedKeyBytes.Length == _adminKeyBytes.Length
               && CryptographicOperations.FixedTimeEquals(providedKeyBytes, _adminKeyBytes);
    }

    private static bool PathStartsWithSegment(string path, string prefix)
    {
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
               || path.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase);
    }
}
