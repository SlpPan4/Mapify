using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class CategorySubmission
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Side Side { get; set; }
    public DateTime SubmittedAt { get; set; }

    public CategorySubmission()
    {
    }

    public CategorySubmission(string name, Side side)
    {
        Name = name;
        Side = side;
    }

    public CategorySubmission(int id, string name, Side side, DateTime submittedAt)
    {
        Id = id;
        Name = name;
        Side = side;
        SubmittedAt = submittedAt;
    }
}
