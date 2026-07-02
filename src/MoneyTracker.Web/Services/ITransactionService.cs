using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetByUserAsync(string userId, int? month, int? year, int? categoryId, string? keyword = null);
    Task<Transaction?> GetByIdAsync(int id, string userId);
    Task CreateAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(int id, string userId);
    Task<decimal> GetMonthlyIncomeAsync(string userId, int month, int year);
    Task<decimal> GetMonthlyExpenseAsync(string userId, int month, int year);
    Task<List<Transaction>> GetRecentAsync(string userId, int count);
    Task<List<(int CategoryId, string Name, decimal Amount)>> GetExpenseByCategoryAsync(string userId, int month, int year);
    Task<List<(int Month, decimal Income, decimal Expense)>> GetMonthlyTrendAsync(string userId, int year);
}
