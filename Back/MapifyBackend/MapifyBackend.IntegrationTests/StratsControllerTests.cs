using System.Net;
using System.Net.Http.Json;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class StratsControllerTests : ControllerTestsBase
{
    public StratsControllerTests()
    {
    }

    [Fact]
    public async Task GetAll_ReturnsStratsList()
    {
        var response = await Client.GetAsync("/api/strats");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetAll_InvalidRoute_ReturnsBadRequest()
    {
        var response = await Client.GetAsync("/api/strats/invalid");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsStratDetail()
    {
        var response = await Client.GetAsync("/api/strats/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<StratDetail>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Cool Ash Rush", result.Data.Name);
        Assert.NotNull(result.Data.Map);
        Assert.NotNull(result.Data.Categories);
        Assert.NotNull(result.Data.Operators);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMapById_Existing_ReturnsMap()
    {
        var response = await Client.GetAsync("/api/strats/maps/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<Map>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal("Oregon", result.Data.Name);
    }

    [Fact]
    public async Task GetMapById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/maps/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated()
    {
        var request = new StratRequest
        {
            Name = "Test Strat",
            VideoUrl = "https://example.com",
            MapName = "Oregon"
        };

        var response = await Client.PostAsJsonAsync("/api/strats", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strategy added!", result.Message);
    }

    [Fact]
    public async Task Create_Invalid_ReturnsBadRequest()
    {
        var request = new StratRequest
        {
            Name = "",
            VideoUrl = "",
            MapName = "Oregon"
        };

        var response = await Client.PostAsJsonAsync("/api/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStrat_Existing_ReturnsSuccess()
    {
        var response = await Client.DeleteAsync("/api/strats/1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strategy deleted!", result.Message);
    }

    [Fact]
    public async Task DeleteStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/strats/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AssignStratToCategory_Valid_ReturnsSuccess()
    {
        var response = await Client.PostAsync("/api/strats/assign/strat/1/category/2", null);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strat assigned", result.Message);
    }

    [Fact]
    public async Task AssignStratToCategory_Invalid_ReturnsBadRequest()
    {
        var response = await Client.PostAsync("/api/strats/assign/strat/9999/category/1", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStratsByCategory_Existing_ReturnsStrats()
    {
        var response = await Client.GetAsync("/api/strats/category/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, s => s.Id == 1);
    }

    [Fact]
    public async Task GetStratsByCategory_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/category/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetStratsByMap_Existing_ReturnsStrats()
    {
        var response = await Client.GetAsync("/api/strats/bymap/7");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, s => s.Id == 1);
    }

    [Fact]
    public async Task GetStratsByMap_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/bymap/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetStratsByOperator_Existing_ReturnsStrats()
    {
        var response = await Client.GetAsync("/api/strats/byoperator/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, s => s.Id == 1);
    }

    [Fact]
    public async Task GetStratsByOperator_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/byoperator/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMapIdByName_Existing_ReturnsId()
    {
        var response = await Client.GetAsync("/api/strats/maps/byname/Oregon");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<int>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Data);
    }

    [Fact]
    public async Task GetMapIdByName_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/strats/maps/byname/NonExistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStrat_Valid_ReturnsSuccess()
    {
        var request = new StratUpdateRequest
        {
            Name = "Updated Strat",
            VideoUrl = "https://updated.example.com",
            MapName = "Bank"
        };

        var response = await Client.PutAsJsonAsync("/api/strats/1", request);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strategy updated!", result.Message);
    }

    [Fact]
    public async Task UpdateStrat_NonExisting_ReturnsNotFound()
    {
        var request = new StratUpdateRequest
        {
            Name = "Updated Strat",
            VideoUrl = "https://updated.example.com",
            MapName = "Bank"
        };

        var response = await Client.PutAsJsonAsync("/api/strats/9999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchStrat_Valid_ReturnsSuccess()
    {
        var request = new StratPatchRequest
        {
            Description = "Patched description"
        };

        var response = await Client.PatchAsJsonAsync("/api/strats/1", request);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strategy updated!", result.Message);
    }

    [Fact]
    public async Task PatchStrat_NonExisting_ReturnsNotFound()
    {
        var request = new StratPatchRequest
        {
            Description = "Patched description"
        };

        var response = await Client.PatchAsJsonAsync("/api/strats/9999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithNameFilter_ReturnsMatchingStrats()
    {
        var response = await Client.GetAsync("/api/strats?name=Ash");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, s => Assert.Contains("Ash", s.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAll_WithMapFilter_ReturnsMatchingStrats()
    {
        var response = await Client.GetAsync("/api/strats?mapId=7");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Strat>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, s => s.Id == 1);
    }

    [Fact]
    public async Task Create_WithLowercaseMapName_ReturnsCreated()
    {
        var request = new StratRequest
        {
            Name = "Lowercase Map Test",
            VideoUrl = "https://example.com",
            MapName = "oregon"
        };

        var response = await Client.PostAsJsonAsync("/api/strats", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
