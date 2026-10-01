namespace MapifyBackend.Utility.DTOs;

/// <summary>
/// Request model for creating or renaming a map.
/// </summary>
public class MapRequest
{
    public string Name { get; set; } = null!;
}
