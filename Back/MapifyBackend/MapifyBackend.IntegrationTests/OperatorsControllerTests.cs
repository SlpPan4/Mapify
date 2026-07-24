using System.Net;
using System.Net.Http.Json;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.Enums;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class OperatorsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task GetAll_ReturnsOperatorsList()
    {
        var response = await Client.GetAsync("/api/operators");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<Operator>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOperator()
    {
        var response = await Client.GetAsync("/api/operators/1");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<Operator>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Ash", result.Data.Name);
        Assert.Equal(Side.Attack, result.Data.Side);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/operators/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetIdByName_Existing_ReturnsId()
    {
        var response = await Client.GetAsync("/api/operators/by-name/Ash");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetIdByName_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/operators/by-name/NonExistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AssignToStrat_Valid_ReturnsSuccess()
    {
        var response = await Client.PostAsync("/api/operators/1/assign/2", null);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Operator assigned successfully", result.Message);
    }

    [Fact]
    public async Task AssignToStrat_Invalid_ReturnsBadRequest()
    {
        var response = await Client.PostAsync("/api/operators/1/assign/9999", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveFromStrat_Existing_ReturnsSuccess()
    {
        var response = await Client.DeleteAsync("/api/operators/1/remove/1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Operator removed from strategy", result.Message);
    }

    [Fact]
    public async Task RemoveFromStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/operators/1/remove/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
