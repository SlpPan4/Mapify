using MapifyBackend.Utility.DataNormalizingHelpers;

namespace MapifyBackend.Utility.DTOs;

using static StringHelper;

public class StratSubmissionRequest
{
    private string _mapName = string.Empty;
    public string Name { get; set; }
    public string VideoUrl { get; set; }
    public string? Description { get; set; }
    public List<int> CategoryIds { get; set; } = [];
    public List<int> OperatorIds { get; set; } = [];

    public string MapName
    {
        get => _mapName;
        set => _mapName = Capitalize(value);
    }
}
