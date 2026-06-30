namespace MapifyBackend.database_files;

public class Strat
{
    public int Id { get; private set; } //ID of the strategy in the database
    public string Name { get; private set; } //Name of the strat
    public string VideoUrl { get; private set; } //URL for the video of the strat
    public int MapId { get; private set; } //ID of the map
    public string? Description { get; private set; }

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
