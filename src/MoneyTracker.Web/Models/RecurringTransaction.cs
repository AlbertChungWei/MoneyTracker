using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.Web.Models;

public class RecurringTransaction
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public TransactionType Type { get; set; }

    [Range(1, 28)]
    public int DayOfMonth { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public int LastGeneratedYear { get; set; }
    public int LastGeneratedMonth { get; set; }
}
