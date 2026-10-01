using System.Net;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class MapsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsMapsList()
    {
        var response = await Client.GetAsync("/api/maps");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Map>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Contains(result.Data, m => m.Name == "Oregon");
    }

    [Fact]
    public async Task GetById_Existing_ReturnsMapWithBombsites()
    {
        var response = await Client.GetAsync("/api/maps/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<Map>();
        Assert.NotNull(result?.Data);
        Assert.Equal("Oregon", result.Data.Name);
        Assert.NotNull(result.Data.Bombsites);
        Assert.Equal(5, result.Data.Bombsites.Count);
        Assert.Contains(result.Data.Bombsites, b => b.Name == "Site A");
        Assert.Contains(result.Data.Bombsites, b => b.Name == "All");
        Assert.All(result.Data.Bombsites, b => Assert.Equal(1, b.MapId));
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/maps/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
