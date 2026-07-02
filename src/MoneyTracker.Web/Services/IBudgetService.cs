using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public interface IBudgetService
{
    Task<List<Budget>> GetByUserAsync(string userId);
    Task SetAsync(string userId, int categoryId, decimal amount);
    Task DeleteAsync(int id, string userId);
}
