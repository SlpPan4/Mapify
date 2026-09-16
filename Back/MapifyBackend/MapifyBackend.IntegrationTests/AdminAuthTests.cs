using System.Net;
using System.Net.Http.Json;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Xunit;

namespace MapifyBackend.IntegrationTests;

/// <summary>
/// Verifies the API-key middleware: admin routes and mutating requests require
/// the X-Api-Key header, while public reads and public submissions pass through.
/// Uses a keyless client (<see cref="Factory"/>.CreateClient()) where needed;
/// the base <see cref="ControllerTestsBase.Client"/> already carries the test key.
/// </summary>
public class AdminAuthTests : ControllerTestsBase
{
    private const string AdminPendingStratsUrl = "/api/submissions/admin/strats";

    [Fact]
    public async Task AdminEndpoint_WithoutKey_Returns401()
    {
        var keyless = Factory.CreateClient();

        var response = await keyless.GetAsync(AdminPendingStratsUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_WithWrongKey_Returns401()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiKeyAuthMiddleware.HeaderName, "wrong-key");

        var response = await client.GetAsync(AdminPendingStratsUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminEndpoint_WithValidKey_Succeeds()
    {
        var response = await Client.GetAsync(AdminPendingStratsUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MutatingEndpoint_WithoutKey_Returns401()
    {
        var keyless = Factory.CreateClient();

        var response = await keyless.DeleteAsync("/api/strats/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PublicGet_WithoutKey_Succeeds()
    {
        var keyless = Factory.CreateClient();

        var response = await keyless.GetAsync("/api/maps");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PublicSubmission_WithoutKey_IsNotBlocked()
    {
        var keyless = Factory.CreateClient();
        var request = new StratSubmissionRequest
        {
            Name = "Keyless submission",
            VideoUrl = "https://example.com/video",
            MapId = 1
        };

        var response = await keyless.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
