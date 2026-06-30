using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Data;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public class TransactionService(AppDbContext db) : ITransactionService
{
    public async Task<List<Transaction>> GetByUserAsync(
        string userId, int? month, int? year, int? categoryId)
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
}
