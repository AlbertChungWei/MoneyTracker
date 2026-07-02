using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public interface IRecurringTransactionService
{
    Task<List<RecurringTransaction>> GetByUserAsync(string userId);
    Task CreateAsync(RecurringTransaction recurring);
    Task ToggleActiveAsync(int id, string userId);
    Task DeleteAsync(int id, string userId);
    Task<int> GenerateForCurrentMonthAsync(string userId);
}
