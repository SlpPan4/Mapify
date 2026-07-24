using MapifyBackend.Utility.DataNormalizingHelpers;

namespace MapifyBackend.Utility.DTOs;
using static StringHelper;

/// <summary>
/// Request model for fully replacing an existing strategy.
/// </summary>
public class StratUpdateRequest
{
    private string _mapName = string.Empty;

    public string Name { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public string? Description { get; set; }

    public string MapName
    {
        get => _mapName;
        set => _mapName = Capitalize(value);
    }
}
