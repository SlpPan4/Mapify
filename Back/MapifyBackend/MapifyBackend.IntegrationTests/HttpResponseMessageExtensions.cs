using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MapifyBackend.Utility.Api;

namespace MapifyBackend.IntegrationTests;

public static class HttpResponseMessageExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<ApiResponse<T>?> ReadApiResponseAsync<T>(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions);
    }
}
