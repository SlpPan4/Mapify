using MapifyBackend.Utility.DataNormalizingHelpers;

namespace MapifyBackend.Utility.DTOs;
using static StringHelper;

/// <summary>
/// Request model for partially updating a strategy.
/// All properties are nullable; only provided values are updated.
/// </summary>
public class StratPatchRequest
{
    private string? _mapName;

    public string? Name { get; set; }
    public string? VideoUrl { get; set; }
    public string? Description { get; set; }

    public string? MapName
    {
        get => _mapName;
        set => _mapName = value is null ? null : Capitalize(value);
    }
}
