namespace MapifyBackend.database_files;

public class StratService
{
    private DatabaseService _dbService;

    public StratService(DatabaseService databaseService)
    {
        _dbService = databaseService;
    }
    
    public void CreateStrat(string name, string videoUrl, string mapName)
    {
        int mapId = _dbService.GetMapIdByName(mapName);

        Strat strat = new Strat(name, videoUrl, mapId);
        _dbService.AddStrat(strat);
    }

    //returns a result of GetStrat(int id) method from DatabaseService
    public Strat? GetStratId(int id)
    {
        return _dbService.GetStrat(id);
    }

    //returns a result of GetAllStrats() method from DatabaseService
    public List<Strat> GetAllStrats()
    {
        return _dbService.GetAllStrats();
    }

    //validates if there is a strat by given ID, returns false if there is none, or calls DeleteStrat from DBService 
    public bool DeleteStrat(int id)
    {
        var strat = GetStratId(id);
        if (strat == null) return false;

        _dbService.DeleteStrat(id);
        return true;
    }

    public void AssignStratToCategory(int stratId, int categoryId)
    {
        _dbService.AssignStratToCategory(stratId, categoryId);
    }

    public List<Strat>? GetStratsByCategory(int categoryId)
    {
        return _dbService.GetStratsByCategory(categoryId);
    }
}