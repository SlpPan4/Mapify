using MapifyBackend.Utility.Enums;

namespace MapifyBackend.database_files;

public class CategoryService
{
    private DatabaseService _db;

    public CategoryService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _db.GetCategoryById(id);
    }

    public async Task<List<Category>> GetAllCategories()
    {
        return await _db.GetAllCategories();
    }

    public async Task<int> AddCategory(string name, Side side)
    {
        Category category = new Category(name, side);
        return await _db.AddCategory(category);
    }

    public async Task<bool> DeleteCategory(int id)
    {
        if (await GetCategoryById(id) == null) return false;

        await _db.DeleteCategory(id);
        return true;
    }

    public async Task<string?> GetCategoryNameById(int id)
    {
        return await _db.GetCategoryNameById(id);
    }
}
