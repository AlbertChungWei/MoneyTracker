using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Data;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<List<Category>> GetByUserAsync(string userId) =>
        await db.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<List<Category>> GetByUserAndTypeAsync(string userId, TransactionType type) =>
        await db.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.Type == type)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task CreateAsync(Category category)
    {
        db.Categories.Add(category);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category is not null)
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }
    }
}
