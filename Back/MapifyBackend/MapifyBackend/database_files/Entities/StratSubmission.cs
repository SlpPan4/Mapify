namespace MapifyBackend.database_files;

public class StratSubmission
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string VideoUrl { get; private set; }
    public int MapId { get; private set; }
    public string? Description { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public List<int> CategoryIds { get; set; } = [];
    public List<int> OperatorIds { get; set; } = [];

    public StratSubmission()
    {
    }

    public StratSubmission(
        string name,
        string videoUrl,
        int mapId,
        string? description,
        List<int> categoryIds,
        List<int> operatorIds)
    {
        Name = name;
        VideoUrl = videoUrl;
        MapId = mapId;
        Description = description;
        CategoryIds = categoryIds;
        OperatorIds = operatorIds;
    }
}
