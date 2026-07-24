using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MapifyBackend.database_files;
using MapifyBackend.Utility.Api;
using MapifyBackend.Utility.DTOs;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class SubmissionsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task SubmitStrat_Valid_ReturnsSuccess()
    {
        var request = CreateValidStratSubmissionRequest();

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SubmitStrat_Invalid_ReturnsBadRequest()
    {
        var request = new StratSubmissionRequest
        {
            Name = "",
            VideoUrl = "",
            MapName = "Oregon",
            CategoryIds = [],
            OperatorIds = []
        };

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SubmitStrat_MapNotFound_ReturnsNotFound()
    {
        var request = CreateValidStratSubmissionRequest();
        request.MapName = "NonExistent";

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitCategory_Valid_ReturnsSuccess()
    {
        var request = new CategorySubmissionRequest
        {
            Name = $"TestSubmissionCategory_{Guid.NewGuid()}",
            Side = "Attack"
        };

        var response = await Client.PostAsJsonAsync("/api/submissions/categories", request);

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task SubmitCategory_InvalidSide_ReturnsBadRequest()
    {
        var request = new CategorySubmissionRequest
        {
            Name = "Test Category",
            Side = "Invalid"
        };

        var response = await Client.PostAsJsonAsync("/api/submissions/categories", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPendingStrats_AfterSubmission_ReturnsSubmittedStrat()
    {
        var request = CreateValidStratSubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/strats", request);
        submitResponse.EnsureSuccessStatusCode();

        var response = await Client.GetAsync("/api/submissions/admin/strats");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<StratSubmission>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
    }

    [Fact]
    public async Task GetPendingStrat_Existing_ReturnsStratSubmission()
    {
        var request = CreateValidStratSubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/strats", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.GetAsync($"/api/submissions/admin/strats/{submissionId}");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<StratSubmission>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(submissionId, result.Data.Id);
    }

    [Fact]
    public async Task GetPendingStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/submissions/admin/strats/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApproveStrat_Existing_ReturnsStratId()
    {
        var request = CreateValidStratSubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/strats", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.PostAsync($"/api/submissions/admin/strats/{submissionId}/approve", null);

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ApproveStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.PostAsync("/api/submissions/admin/strats/9999/approve", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RejectStrat_Existing_ReturnsSuccess()
    {
        var request = CreateValidStratSubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/strats", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.DeleteAsync($"/api/submissions/admin/strats/{submissionId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Strategy submission rejected", result.Message);
    }

    [Fact]
    public async Task RejectStrat_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/submissions/admin/strats/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPendingCategories_AfterSubmission_ReturnsSubmittedCategory()
    {
        var request = CreateValidCategorySubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/categories", request);
        submitResponse.EnsureSuccessStatusCode();

        var response = await Client.GetAsync("/api/submissions/admin/categories");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<List<CategorySubmission>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, c => c.Name == request.Name);
    }

    [Fact]
    public async Task GetPendingCategory_Existing_ReturnsCategorySubmission()
    {
        var request = CreateValidCategorySubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/categories", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.GetAsync($"/api/submissions/admin/categories/{submissionId}");

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<CategorySubmission>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(submissionId, result.Data.Id);
    }

    [Fact]
    public async Task GetPendingCategory_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/submissions/admin/categories/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApproveCategory_Existing_ReturnsCategoryId()
    {
        var request = CreateValidCategorySubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/categories", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.PostAsync($"/api/submissions/admin/categories/{submissionId}/approve", null);

        response.EnsureSuccessStatusCode();
        var result = await response.ReadApiResponseAsync<object>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ApproveCategory_NonExisting_ReturnsNotFound()
    {
        var response = await Client.PostAsync("/api/submissions/admin/categories/9999/approve", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RejectCategory_Existing_ReturnsSuccess()
    {
        var request = CreateValidCategorySubmissionRequest();
        var submitResponse = await Client.PostAsJsonAsync("/api/submissions/categories", request);
        submitResponse.EnsureSuccessStatusCode();
        var submitResult = await submitResponse.ReadApiResponseAsync<object>();
        Assert.NotNull(submitResult);
        var submitData = (JsonElement)submitResult.Data!;
        int submissionId = submitData.GetProperty("submissionId").GetInt32();

        var response = await Client.DeleteAsync($"/api/submissions/admin/categories/{submissionId}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.Equal("Category submission rejected", result.Message);
    }

    [Fact]
    public async Task RejectCategory_NonExisting_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync("/api/submissions/admin/categories/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitStrat_MismatchedCategorySides_ReturnsBadRequest()
    {
        var request = CreateValidStratSubmissionRequest();
        request.CategoryIds = [1, 5]; // Attack + Defense

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SubmitStrat_MismatchedOperatorSides_ReturnsBadRequest()
    {
        var request = CreateValidStratSubmissionRequest();
        request.OperatorIds = [1, 5]; // Attack + Defense

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SubmitStrat_CategoryOperatorSideMismatch_ReturnsBadRequest()
    {
        var request = CreateValidStratSubmissionRequest();
        request.CategoryIds = [1];   // Attack
        request.OperatorIds = [5];   // Defense

        var response = await Client.PostAsJsonAsync("/api/submissions/strats", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static StratSubmissionRequest CreateValidStratSubmissionRequest()
    {
        return new StratSubmissionRequest
        {
            Name = $"TestSubmissionStrat_{Guid.NewGuid()}",
            VideoUrl = "https://example.com",
            MapName = "Oregon",
            Description = "Test description",
            CategoryIds = [1],
            OperatorIds = [1]
        };
    }

    private static CategorySubmissionRequest CreateValidCategorySubmissionRequest()
    {
        return new CategorySubmissionRequest
        {
            Name = $"TestSubmissionCategory_{Guid.NewGuid()}",
            Side = "Attack"
        };
    }
}
