using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Data;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public class TransactionService(AppDbContext db) : ITransactionService
{
    public async Task<List<Transaction>> GetByUserAsync(
        string userId, int? month, int? year, int? categoryId, string? keyword = null)
    {
        var query = db.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (year.HasValue)
            query = query.Where(t => t.Date.Year == year.Value);
        if (month.HasValue)
            query = query.Where(t => t.Date.Month == month.Value);
        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(t => t.Description.Contains(keyword));

        return await query.OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id, string userId) =>
        await db.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task CreateAsync(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction is not null)
        {
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetMonthlyIncomeAsync(string userId, int month, int year) =>
        await db.Transactions
            .Where(t => t.UserId == userId
                     && t.Type == TransactionType.Income
                     && t.Date.Month == month
                     && t.Date.Year == year)
            .SumAsync(t => t.Amount);

    public async Task<decimal> GetMonthlyExpenseAsync(string userId, int month, int year) =>
        await db.Transactions
            .Where(t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Date.Month == month
                     && t.Date.Year == year)
            .SumAsync(t => t.Amount);

    public async Task<List<Transaction>> GetRecentAsync(string userId, int count) =>
        await db.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .Take(count)
            .ToListAsync();

    public async Task<List<(int CategoryId, string Name, decimal Amount)>> GetExpenseByCategoryAsync(
        string userId, int month, int year)
    {
        var raw = await db.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId
                     && t.Type == TransactionType.Expense
                     && t.Date.Month == month
                     && t.Date.Year == year)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Amount = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.Amount)
            .ToListAsync();

        if (raw.Count == 0) return [];

        var catIds = raw.Select(r => r.CategoryId).ToList();
        var cats = await db.Categories.AsNoTracking()
            .Where(c => catIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        return raw
            .Select(r => (r.CategoryId, cats.GetValueOrDefault(r.CategoryId, "未知"), r.Amount))
            .ToList();
    }

    public async Task<List<(int Month, decimal Income, decimal Expense)>> GetMonthlyTrendAsync(
        string userId, int year)
    {
        var data = await db.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date.Year == year)
            .Select(t => new { t.Date.Month, t.Type, t.Amount })
            .ToListAsync();

        return Enumerable.Range(1, 12).Select(m =>
        (
            m,
            data.Where(t => t.Month == m && t.Type == TransactionType.Income).Sum(t => t.Amount),
            data.Where(t => t.Month == m && t.Type == TransactionType.Expense).Sum(t => t.Amount)
        )).ToList();
    }
}
