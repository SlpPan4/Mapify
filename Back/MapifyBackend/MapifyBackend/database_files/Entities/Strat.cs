namespace MapifyBackend.database_files;

public class Strat
{
    public int Id { get; set; } //ID of the strategy in the database
    public string Name { get; set; } = null!; //Name of the strat
    public string VideoUrl { get; set; } = null!; //URL for the video of the strat
    public int MapId { get; set; } //ID of the map
    public string? Description { get; set; }
    public int? BombsiteId { get; set; } //ID of the bombsite, if the strat targets one

    public Strat(string stratName, string videoUrl, int mapId)
    {
        Name = stratName;
        VideoUrl = videoUrl;
        MapId = mapId;
    }

    public Strat(string stratName, string videoUrl, int mapId, string? description)
    {
        Name = stratName;
        VideoUrl = videoUrl;
        MapId = mapId;
        Description = description;
    }
    
    public Strat(int id, string stratName, string videoUrl, int mapId, string? description)
    {
        Id = id;
        Name = stratName;
        VideoUrl = videoUrl;
        MapId = mapId;
        Description = description;
    }

    public Strat()
    {
        
    }

    public void ChangeStratName(string stratName)
    {
        Name = stratName;
    }

    public void ChangeStratVideoUrl(string url)
    {
        VideoUrl = url;
    }

    public void SetId(int id)
    {
        Id = id;
    }
}
