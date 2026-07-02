using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Data;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public class RecurringTransactionService(AppDbContext db) : IRecurringTransactionService
{
    public async Task<List<RecurringTransaction>> GetByUserAsync(string userId) =>
        await db.RecurringTransactions
            .AsNoTracking()
            .Include(r => r.Category)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.DayOfMonth)
            .ToListAsync();

    public async Task CreateAsync(RecurringTransaction recurring)
    {
        db.RecurringTransactions.Add(recurring);
        await db.SaveChangesAsync();
    }

    public async Task ToggleActiveAsync(int id, string userId)
    {
        var item = await db.RecurringTransactions
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (item is not null)
        {
            item.IsActive = !item.IsActive;
            await db.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var item = await db.RecurringTransactions
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (item is not null)
        {
            db.RecurringTransactions.Remove(item);
            await db.SaveChangesAsync();
        }
    }

    public async Task<int> GenerateForCurrentMonthAsync(string userId)
    {
        var now = DateTime.Now;
        var items = await db.RecurringTransactions
            .Where(r => r.UserId == userId
                     && r.IsActive
                     && !(r.LastGeneratedYear == now.Year && r.LastGeneratedMonth == now.Month))
            .ToListAsync();

        if (items.Count == 0) return 0;

        foreach (var item in items)
        {
            var day = Math.Min(item.DayOfMonth, DateTime.DaysInMonth(now.Year, now.Month));
            db.Transactions.Add(new Transaction
            {
                UserId = userId,
                CategoryId = item.CategoryId,
                Amount = item.Amount,
                Description = item.Description,
                Type = item.Type,
                Date = new DateTime(now.Year, now.Month, day),
                Notes = "（週期性自動建立）"
            });
            item.LastGeneratedYear = now.Year;
            item.LastGeneratedMonth = now.Month;
        }

        await db.SaveChangesAsync();
        return items.Count;
    }
}
