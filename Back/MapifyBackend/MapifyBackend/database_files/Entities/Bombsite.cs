namespace MapifyBackend.database_files;

public class Bombsite
{
    public int Id { get; set; }
    public int MapId { get; set; }
    public string Name { get; set; } = null!;

    public Bombsite() { }
    public Bombsite(int id, int mapId, string name) { Id = id; MapId = mapId; Name = name; }

    public void SetId(int id) { Id = id; }
}
