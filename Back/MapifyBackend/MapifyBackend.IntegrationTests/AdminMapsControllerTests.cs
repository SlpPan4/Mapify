using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class AdminMapsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task CreateMap_Valid_ReturnsCreated()
    {
        var request = new MapRequest { Name = "New Test Map" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Map added", result.Message);
    }

    [Fact]
    public async Task CreateMap_DuplicateName_ReturnsBadRequest()
    {
        var request = new MapRequest { Name = "Oregon" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMap_InvalidName_ReturnsBadRequest()
    {
        var request = new MapRequest { Name = "" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMap_Valid_ReturnsSuccess()
    {
        int mapId = await CreateMapAsync("Update Test Map");

        var response = await Client.PutAsJsonAsync($"/api/admin/maps/{mapId}", new MapRequest { Name = "Renamed Map" });

        response.EnsureSuccessStatusCode();
        var getResult = await (await Client.GetAsync($"/api/maps/{mapId}")).ReadApiResponseAsync<Map>();
        Assert.NotNull(getResult?.Data);
        Assert.Equal("Renamed Map", getResult.Data.Name); // names are stored exactly as provided
    }

    [Fact]
    public async Task UpdateMap_NonExisting_ReturnsNotFound()
    {
        var response = await Client.PutAsJsonAsync("/api/admin/maps/9999", new MapRequest { Name = "Whatever" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMap_WithoutStrats_ReturnsSuccess()
    {
        int mapId = await CreateMapAsync("Delete Test Map");

        var response = await Client.DeleteAsync($"/api/admin/maps/{mapId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Map deleted", result.Message);

        var getResponse = await Client.GetAsync($"/api/maps/{mapId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteMap_WithStrats_ReturnsBadRequest()
    {
        // Map 7 (Coastline) is referenced by a seeded strat.
        var response = await Client.DeleteAsync("/api/admin/maps/7");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Contains("strat", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteMap_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/admin/maps/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddBombsite_Valid_ReturnsCreated()
    {
        var request = new BombsiteRequest { Name = "Kitchen" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps/1/bombsites", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Bombsite added", result.Message);

        var mapResult = await (await Client.GetAsync("/api/maps/1")).ReadApiResponseAsync<Map>();
        Assert.NotNull(mapResult?.Data);
        Assert.Contains(mapResult.Data.Bombsites, b => b.Name == "Kitchen");
    }

    [Fact]
    public async Task AddBombsite_DuplicateName_ReturnsBadRequest()
    {
        // Every map is seeded with "Site A".
        var request = new BombsiteRequest { Name = "Site A" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps/1/bombsites", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddBombsite_NonExistingMap_ReturnsNotFound()
    {
        var request = new BombsiteRequest { Name = "Kitchen" };

        var response = await Client.PostAsJsonAsync("/api/admin/maps/9999/bombsites", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBombsite_Valid_ReturnsSuccess()
    {
        // Bombsite 1 is the seeded "Site A" of map 1 (Oregon).
        var response = await Client.PutAsJsonAsync("/api/admin/maps/1/bombsites/1", new BombsiteRequest { Name = "Kitchen" });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Bombsite updated", result.Message);

        var mapResult = await (await Client.GetAsync("/api/maps/1")).ReadApiResponseAsync<Map>();
        Assert.NotNull(mapResult?.Data);
        Assert.Contains(mapResult.Data.Bombsites, b => b.Id == 1 && b.Name == "Kitchen");
    }

    [Fact]
    public async Task UpdateBombsite_WrongMap_ReturnsNotFound()
    {
        // Bombsite 1 belongs to map 1, not map 2.
        var response = await Client.PutAsJsonAsync("/api/admin/maps/2/bombsites/1", new BombsiteRequest { Name = "Kitchen" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBombsite_Valid_ReturnsSuccess()
    {
        var response = await Client.DeleteAsync("/api/admin/maps/1/bombsites/1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Bombsite deleted", result.Message);

        var mapResult = await (await Client.GetAsync("/api/maps/1")).ReadApiResponseAsync<Map>();
        Assert.NotNull(mapResult?.Data);
        Assert.DoesNotContain(mapResult.Data.Bombsites, b => b.Id == 1);
    }

    [Fact]
    public async Task DeleteBombsite_WrongMap_ReturnsNotFound()
    {
        // Bombsite 1 belongs to map 1, not map 2.
        var response = await Client.DeleteAsync("/api/admin/maps/2/bombsites/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateMap_WithoutApiKey_Returns401()
    {
        var keyless = Factory.CreateClient();

        var response = await keyless.PostAsJsonAsync("/api/admin/maps", new MapRequest { Name = "Keyless Map" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBombsite_WithoutApiKey_Returns401()
    {
        var keyless = Factory.CreateClient();

        var response = await keyless.DeleteAsync("/api/admin/maps/1/bombsites/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMap_WithWrongApiKey_Returns401()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiKeyAuthMiddleware.HeaderName, "wrong-key");

        var response = await client.DeleteAsync("/api/admin/maps/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<int> CreateMapAsync(string name)
    {
        var response = await Client.PostAsJsonAsync("/api/admin/maps", new MapRequest { Name = name });
        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        var data = (JsonElement)result!.Data!;
        return data.GetProperty("mapId").GetInt32();
    }
}
