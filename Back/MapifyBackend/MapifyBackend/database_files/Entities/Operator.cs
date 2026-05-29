using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class Operator
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Side Side { get; private set; }

    public Operator()
    {
        
    }

    public Operator(string name, Side side)
    {
        Name = name;
        Side = side;
    }

    public Operator(int id, string name, Side side)
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