namespace MapifyBackend.Utility.Api;

public class SubmissionRateLimitOptions
{
    public int PermitLimit { get; set; } = 10;
    public int WindowSeconds { get; set; } = 60;
}
