using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Services;

public interface ICategoryService
{
    Task<List<Category>> GetByUserAsync(string userId);
    Task<List<Category>> GetByUserAndTypeAsync(string userId, TransactionType type);
    Task CreateAsync(Category category);
    Task DeleteAsync(int id, string userId);
}
