namespace MapifyBackend.database_files;

public class OperatorService
{
    private readonly DatabaseService _db;

    public OperatorService(DatabaseService db)
    {
        _db = db;
    }

    public Operator? GetOperatorById(int id)
        => _db.GetOperatorById(id);

    public List<Operator> GetAllOperators()
        => _db.GetAllOperators();

    public int? GetOperatorIdByName(string name)
        => _db.GetOperatorIdByName(name);

    /// <summary>
    /// Assigns an operator to a strategy.
    /// Returns false if:
    /// - operator does not exist
    /// - relation already exists
    /// - database constraint fails
    /// </summary>
    public bool AssignOperatorToStrat(int stratId, int operatorId)
    {
        // 1. base check for existence of the operator
        if (_db.GetOperatorById(operatorId) == null)
            return false;

        // 2. protection from duplicates
        if (_db.IsOperatorAssignedToStrat(stratId, operatorId))
            return false;

        // 3. insertion attempt
        return _db.AssignOperatorToStrat(stratId, operatorId);
    }

    /// <summary>
    /// Removes operator from strategy.
    /// Returns true if relation existed and was removed.
    /// </summary>
    public bool RemoveOperatorFromStrat(int stratId, int operatorId)
    {
        return _db.RemoveOperatorFromStrat(stratId, operatorId);
    }
}