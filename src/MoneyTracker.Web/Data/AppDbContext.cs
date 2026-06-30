using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Web.Models;

namespace MoneyTracker.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);

            e.HasOne(t => t.User)
             .WithMany(u => u.Transactions)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Category)
             .WithMany(c => c.Transactions)
             .HasForeignKey(t => t.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(t => t.Date);
        });

        builder.Entity<Category>(e =>
        {
            e.HasOne(c => c.User)
             .WithMany(u => u.Categories)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
