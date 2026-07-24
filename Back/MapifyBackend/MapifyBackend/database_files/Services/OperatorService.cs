namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing operators and their assignment to strategies.
/// </summary>
public class OperatorService
{
    private readonly DatabaseService _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperatorService"/> class.
    /// </summary>
    /// <param name="db">The database service used for data persistence.</param>
    public OperatorService(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves an operator by its ID.
    /// </summary>
    /// <param name="id">The operator ID.</param>
    /// <returns>The operator if found; otherwise, null.</returns>
    public async Task<Operator?> GetOperatorById(int id)
        => await _db.GetOperatorById(id);

    /// <summary>
    /// Retrieves all operators in the system.
    /// </summary>
    /// <returns>A list of all operators.</returns>
    public async Task<List<Operator>> GetAllOperators()
        => await _db.GetAllOperators();

    /// <summary>
    /// Retrieves the ID of an operator by its exact name.
    /// </summary>
    /// <param name="name">The operator name.</param>
    /// <returns>The operator ID if found; otherwise, null.</returns>
    public async Task<int?> GetOperatorIdByName(string name)
        => await _db.GetOperatorIdByName(name);

    /// <summary>
    /// Assigns an operator to a strategy.
    /// </summary>
    /// <param name="stratId">The strategy ID.</param>
    /// <param name="operatorId">The operator ID.</param>
    /// <returns>
    /// True if the operator was assigned; false if the operator does not exist,
    /// the relation already exists, or a database constraint fails.
    /// </returns>
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
    /// Removes an operator from a strategy.
    /// </summary>
    /// <param name="stratId">The strategy ID.</param>
    /// <param name="operatorId">The operator ID.</param>
    /// <returns>True if the relation existed and was removed; otherwise, false.</returns>
    public async Task<bool> RemoveOperatorFromStrat(int stratId, int operatorId)
    {
        return await _db.RemoveOperatorFromStrat(stratId, operatorId);
    }
}
