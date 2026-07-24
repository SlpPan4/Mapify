using MapifyBackend.database_files;

namespace MapifyBackend.Utility.DTOs;

/// <summary>
/// Detailed strategy response including map, categories, and operators.
/// </summary>
public class StratDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public string? Description { get; set; }
    public Map Map { get; set; } = null!;
    public List<Category> Categories { get; set; } = [];
    public List<Operator> Operators { get; set; } = [];
}
