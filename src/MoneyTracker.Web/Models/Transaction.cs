using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.Web.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? ReceiptPath { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public TransactionType Type { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}
