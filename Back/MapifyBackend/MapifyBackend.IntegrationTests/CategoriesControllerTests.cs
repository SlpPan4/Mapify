using System.Net;
using System.Net.Http.Json;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using MapifyBackend.Utility.Enums;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class CategoriesControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsCategoriesList()
    {
        var response = await Client.GetAsync("/api/categories");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Category>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsCategory()
    {
        var response = await Client.GetAsync("/api/categories/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<Category>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Rush", result.Data.Name);
        Assert.Equal(Side.Attack, result.Data.Side);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/categories/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Valid_ReturnsSuccess()
    {
        var request = new CategoryRequest
        {
            Name = "Test Category",
            Side = "Defense"
        };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Category added", result.Message);
    }

    [Fact]
    public async Task Create_InvalidSide_ReturnsBadRequest()
    {
        var request = new CategoryRequest
        {
            Name = "Test Category",
            Side = "Invalid"
        };

        var response = await Client.PostAsJsonAsync("/api/categories", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_ReturnsSuccess()
    {
        var response = await Client.DeleteAsync("/api/categories/10");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Category deleted", result.Message);
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/categories/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetName_Existing_ReturnsName()
    {
        var response = await Client.GetAsync("/api/categories/category_name/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetName_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/categories/category_name/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
