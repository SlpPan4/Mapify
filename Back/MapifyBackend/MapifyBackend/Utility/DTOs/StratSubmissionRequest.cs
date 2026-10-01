namespace MapifyBackend.Utility.DTOs;

public class StratSubmissionRequest
{
    public string Name { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public string? Description { get; set; }
    public int MapId { get; set; }
    public int? BombsiteId { get; set; }
    public List<int> CategoryIds { get; set; } = [];
    public List<int> OperatorIds { get; set; } = [];
}
