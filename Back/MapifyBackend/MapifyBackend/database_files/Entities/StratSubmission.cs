namespace MapifyBackend.database_files;

public class StratSubmission
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public int MapId { get; set; }
    public string? Description { get; set; }
    public int? BombsiteId { get; set; }
    public DateTime SubmittedAt { get; set; }
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
        List<int> operatorIds,
        int? bombsiteId = null)
    {
        Name = name;
        VideoUrl = videoUrl;
        MapId = mapId;
        Description = description;
        CategoryIds = categoryIds;
        OperatorIds = operatorIds;
        BombsiteId = bombsiteId;
    }
}
