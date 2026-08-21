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
}
