using System.Net;
using System.Text;
using Xunit;

namespace MapifyBackend.IntegrationTests;

public class SubmissionsRateLimitTests
{
    private static StringContent EmptyJson() => new("{}", Encoding.UTF8, "application/json");

    [Fact]
    public async Task SubmitStrat_ExceedsLimit_Returns429()
    {
        using var factory = new CustomWebApplicationFactory(submissionPermitLimit: 2, submissionWindowSeconds: 60);
        using var client = factory.CreateClient();

        var first = await client.PostAsync("/api/submissions/strats", EmptyJson());
        var second = await client.PostAsync("/api/submissions/strats", EmptyJson());
        var third = await client.PostAsync("/api/submissions/strats", EmptyJson());

        Assert.NotEqual(HttpStatusCode.TooManyRequests, first.StatusCode);
        Assert.NotEqual(HttpStatusCode.TooManyRequests, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
    }

    [Fact]
    public async Task RateLimit_IsSharedAcrossSubmissionEndpoints()
    {
        using var factory = new CustomWebApplicationFactory(submissionPermitLimit: 2, submissionWindowSeconds: 60);
        using var client = factory.CreateClient();

        await client.PostAsync("/api/submissions/strats", EmptyJson());
        await client.PostAsync("/api/submissions/categories", EmptyJson());
        var third = await client.PostAsync("/api/submissions/strats", EmptyJson());

        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
    }

    [Fact]
    public async Task RateLimit_DoesNotAffectOtherEndpoints()
    {
        using var factory = new CustomWebApplicationFactory(submissionPermitLimit: 1, submissionWindowSeconds: 60);
        using var client = factory.CreateClient();

        await client.PostAsync("/api/submissions/strats", EmptyJson());
        var limited = await client.PostAsync("/api/submissions/strats", EmptyJson());
        var getStrats = await client.GetAsync("/api/strats");

        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getStrats.StatusCode);
    }
}
