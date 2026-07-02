using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Data;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public class BudgetService(AppDbContext db) : IBudgetService
{
    public async Task<List<Budget>> GetByUserAsync(string userId) =>
        await db.Budgets
            .AsNoTracking()
            .Include(b => b.Category)
            .Where(b => b.UserId == userId)
            .ToListAsync();

    public async Task SetAsync(string userId, int categoryId, decimal amount)
    {
        var existing = await db.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId);

        if (amount <= 0)
        {
            if (existing is not null)
            {
                db.Budgets.Remove(existing);
                await db.SaveChangesAsync();
            }
            return;
        }

        if (existing is not null)
        {
            existing.Amount = amount;
        }
        else
        {
            db.Budgets.Add(new Budget { UserId = userId, CategoryId = categoryId, Amount = amount });
        }
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var budget = await db.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        if (budget is not null)
        {
            db.Budgets.Remove(budget);
            await db.SaveChangesAsync();
        }
    }
}
