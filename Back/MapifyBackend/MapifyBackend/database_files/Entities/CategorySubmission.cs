using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class CategorySubmission
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Side Side { get; private set; }
    public DateTime SubmittedAt { get; private set; }

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
