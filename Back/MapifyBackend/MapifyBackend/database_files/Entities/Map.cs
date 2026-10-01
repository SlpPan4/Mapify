namespace MapifyBackend.database_files;

public class Map
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    // Populated only by the detailed map query (GetMapWithBombsitesById).
    public List<Bombsite> Bombsites { get; set; } = [];
    
    public Map() { }
    public Map(int id, string name) { Id = id; Name = name; }
    
    public void SetId(int id) { Id = id; }
    public void SetName(string name) { Name = name; }
}