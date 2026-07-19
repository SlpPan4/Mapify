namespace MapifyBackend.database_files;

public class OperatorService
{
    private readonly DatabaseService _db;

    public OperatorService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<Operator?> GetOperatorById(int id)
        => await _db.GetOperatorById(id);

    public async Task<List<Operator>> GetAllOperators()
        => await _db.GetAllOperators();

    public async Task<int?> GetOperatorIdByName(string name)
        => await _db.GetOperatorIdByName(name);

    /// <summary>
    /// Assigns an operator to a strategy.
    /// Returns false if:
    /// - operator does not exist
    /// - relation already exists
    /// - database constraint fails
    /// </summary>
    public async Task<bool> AssignOperatorToStrat(int stratId, int operatorId)
    {
        // 1. base check for existence of the operator
        if (await _db.GetOperatorById(operatorId) == null)
            return false;

        // 2. protection from duplicates
        if (await _db.IsOperatorAssignedToStrat(stratId, operatorId))
            return false;

        // 3. insertion attempt
        return await _db.AssignOperatorToStrat(stratId, operatorId);
    }

    /// <summary>
    /// Removes operator from strategy.
    /// Returns true if relation existed and was removed.
    /// </summary>
    public async Task<bool> RemoveOperatorFromStrat(int stratId, int operatorId)
    {
        return await _db.RemoveOperatorFromStrat(stratId, operatorId);
    }
}
