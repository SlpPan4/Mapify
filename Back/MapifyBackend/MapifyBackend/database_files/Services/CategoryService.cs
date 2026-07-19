using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

/// <summary>
/// Service layer for managing strategy categories.
/// </summary>
public class CategoryService
{
    private readonly DatabaseService _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryService"/> class.
    /// </summary>
    /// <param name="db">The database service used for data persistence.</param>
    public CategoryService(DatabaseService db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a category by its ID.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    public async Task<Category?> GetCategoryById(int id)
    {
        return await _db.GetCategoryById(id);
    }

    /// <summary>
    /// Retrieves all categories in the system.
    /// </summary>
    /// <returns>A list of all categories.</returns>
    public async Task<List<Category>> GetAllCategories()
    {
        return await _db.GetAllCategories();
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="side">The category side (Attack or Defense).</param>
    /// <returns>The ID of the newly created category.</returns>
    public async Task<int> AddCategory(string name, Side side)
    {
        Category category = new Category(name, side);
        return await _db.AddCategory(category);
    }

    /// <summary>
    /// Deletes a category by its ID.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <returns>True if the category was found and deleted; otherwise, false.</returns>
    public async Task<bool> DeleteCategory(int id)
    {
        if (await GetCategoryById(id) == null) return false;

        await _db.DeleteCategory(id);
        return true;
    }

    /// <summary>
    /// Retrieves the name of a category by its ID.
    /// </summary>
    /// <param name="id">The category ID.</param>
    /// <returns>The category name if found; otherwise, null.</returns>
    public async Task<string?> GetCategoryNameById(int id)
    {
        return await _db.GetCategoryNameById(id);
    }
}
