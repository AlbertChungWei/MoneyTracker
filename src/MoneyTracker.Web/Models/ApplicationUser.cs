using Microsoft.AspNetCore.Identity;

namespace MoneyTracker.Web.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Transaction> Transactions { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];
}
