namespace MapifyBackend.Utility.DTOs;

/// <summary>
/// Request model for creating or renaming a bombsite.
/// </summary>
public class BombsiteRequest
{
    public string Name { get; set; } = null!;
}
