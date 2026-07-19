using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Side Side { get; set; }

    public Category()
    {
        
    }

    public Category(string name, Side side)
    {
        Name = name;
        Side = side;
    }

    public Category(int id, string name, Side side)
    {
        Id = id;
        Name = name;
        Side = side;
    }

    public void SetId(int id)
    {

        Id = id;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetSide(Side side)
    {
        Side = side;
    }
}