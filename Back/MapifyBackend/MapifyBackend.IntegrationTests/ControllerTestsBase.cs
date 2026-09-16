using System.Net.Http.Json;
using MapifyBackend.Utility.Api;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public abstract class ControllerTestsBase : IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ControllerTestsBase()
    {
        Factory = new CustomWebApplicationFactory();
        Client = Factory.CreateClient();
        Client.DefaultRequestHeaders.Add(ApiKeyAuthMiddleware.HeaderName, CustomWebApplicationFactory.TestAdminKey);
    }

    protected async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response) where T : class?
    {
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return apiResponse?.Data;
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}
