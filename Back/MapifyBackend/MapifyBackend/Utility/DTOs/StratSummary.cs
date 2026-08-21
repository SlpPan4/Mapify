using MapifyBackend.database_files;

namespace MapifyBackend.Utility.DTOs;

/// <summary>
/// Strategy list item enriched with map, side, categories, and operators.
/// </summary>
public class StratSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public string? Description { get; set; }
    public Map Map { get; set; } = null!;
    public string Side { get; set; } = null!;
    public List<Category> Categories { get; set; } = [];
    public List<Operator> Operators { get; set; } = [];
}
