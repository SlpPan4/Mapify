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

    [Fact]
    public async Task GetSummary_ReturnsEnrichedStrats()
    {
        var response = await Client.GetAsync("/api/strats/summary");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<StratSummary>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);

        var strat = result.Data.FirstOrDefault(s => s.Id == 1);
        Assert.NotNull(strat);
        Assert.Equal("Cool Ash Rush", strat.Name);
        Assert.NotNull(strat.Map);
        Assert.Equal("Coastline", strat.Map.Name);
        Assert.Contains(strat.Categories, c => c.Name == "Rush");
        Assert.Contains(strat.Operators, o => o.Name == "Ash");
    }

    [Fact]
    public async Task RemoveCategoryFromStrat_Existing_ReturnsSuccess()
    {
        var response = await Client.DeleteAsync("/api/strats/1/categories/1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Category removed from strat", result.Message);
    }

    [Fact]
    public async Task RemoveCategoryFromStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/strats/1/categories/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDescription_SavesDescription()
    {
        var request = new StratRequest
        {
            Name = "Description Test Strat",
            VideoUrl = "https://example.com/description",
            MapName = "Oregon",
            Description = "This description should be persisted"
        };

        var createResponse = await Client.PostAsJsonAsync("/api/strats", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(createResult);
        Assert.NotNull(createResult.Data);

        int stratId = ((System.Text.Json.JsonElement)createResult.Data).GetProperty("stratId").GetInt32();

        var getResponse = await Client.GetAsync($"/api/strats/{stratId}");
        getResponse.EnsureSuccessStatusCode();
        var getResult = await getResponse.ReadApiResponseAsync<StratDetail>();
        Assert.NotNull(getResult);
        Assert.NotNull(getResult.Data);
        Assert.Equal("This description should be persisted", getResult.Data.Description);
    }

    [Fact]
    public async Task Create_WithBombsiteId_PersistsBombsite()
    {
        // Bombsite 1 is the seeded "Site A" of map 1 (Oregon).
        var request = new StratRequest
        {
            Name = "Bombsite Test Strat",
            VideoUrl = "https://example.com/bombsite",
            MapName = "Oregon",
            BombsiteId = 1
        };

        var createResponse = await Client.PostAsJsonAsync("/api/strats", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(createResult?.Data);
        int stratId = ((System.Text.Json.JsonElement)createResult.Data).GetProperty("stratId").GetInt32();

        var getResult = await (await Client.GetAsync($"/api/strats/{stratId}")).ReadApiResponseAsync<StratDetail>();
        Assert.NotNull(getResult?.Data);
        Assert.NotNull(getResult.Data.Bombsite);
        Assert.Equal(1, getResult.Data.Bombsite.Id);
        Assert.Equal("Site A", getResult.Data.Bombsite.Name);
        Assert.Equal(getResult.Data.Map.Id, getResult.Data.Bombsite.MapId);
    }

    [Fact]
    public async Task Create_WithBombsiteFromAnotherMap_ReturnsBadRequest()
    {
        // Bombsite 6 belongs to map 2, not to map 1 (Oregon).
        var request = new StratRequest
        {
            Name = "Wrong Map Bombsite",
            VideoUrl = "https://example.com/bombsite",
            MapName = "Oregon",
            BombsiteId = 6
        };

        var response = await Client.PostAsJsonAsync("/api/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithNonExistingBombsite_ReturnsNotFound()
    {
        var request = new StratRequest
        {
            Name = "Missing Bombsite",
            VideoUrl = "https://example.com/bombsite",
            MapName = "Oregon",
            BombsiteId = 999999
        };

        var response = await Client.PostAsJsonAsync("/api/strats", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStrat_WithBombsiteId_PersistsBombsite()
    {
        // Strat 1 is on map 7 (Coastline); bombsite 31 is its seeded "Site A".
        var request = new StratUpdateRequest
        {
            Name = "Updated Strat",
            VideoUrl = "https://updated.example.com",
            MapName = "Coastline",
            BombsiteId = 31
        };

        var response = await Client.PutAsJsonAsync("/api/strats/1", request);

        response.EnsureSuccessStatusCode();

        var getResult = await (await Client.GetAsync("/api/strats/1")).ReadApiResponseAsync<StratDetail>();
        Assert.NotNull(getResult?.Data?.Bombsite);
        Assert.Equal(31, getResult.Data.Bombsite.Id);
    }

    [Fact]
    public async Task PatchStrat_WithBombsiteId_PersistsBombsite()
    {
        // Strat 1 is on map 7 (Coastline); bombsite 32 is its seeded "Site B".
        var request = new StratPatchRequest
        {
            BombsiteId = 32
        };

        var response = await Client.PatchAsJsonAsync("/api/strats/1", request);

        response.EnsureSuccessStatusCode();

        var getResult = await (await Client.GetAsync("/api/strats/1")).ReadApiResponseAsync<StratDetail>();
        Assert.NotNull(getResult?.Data?.Bombsite);
        Assert.Equal(32, getResult.Data.Bombsite.Id);
        Assert.Equal("Site B", getResult.Data.Bombsite.Name);
    }

    [Fact]
    public async Task GetSummary_IncludesBombsite()
    {
        var request = new StratRequest
        {
            Name = "Summary Bombsite Strat",
            VideoUrl = "https://example.com/summary",
            MapName = "Oregon",
            BombsiteId = 1
        };

        var createResponse = await Client.PostAsJsonAsync("/api/strats", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(createResult?.Data);
        int stratId = ((System.Text.Json.JsonElement)createResult.Data).GetProperty("stratId").GetInt32();

        var response = await Client.GetAsync("/api/strats/summary");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<StratSummary>>();
        Assert.NotNull(result?.Data);

        var strat = result.Data.FirstOrDefault(s => s.Id == stratId);
        Assert.NotNull(strat);
        Assert.NotNull(strat.Bombsite);
        Assert.Equal(1, strat.Bombsite.Id);
        Assert.Equal("Site A", strat.Bombsite.Name);
    }
}
