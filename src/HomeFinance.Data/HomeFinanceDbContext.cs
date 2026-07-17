using HomeFinance.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeFinance.Data;

public sealed class HomeFinanceDbContext : IdentityDbContext<ApplicationUser>
{
    public HomeFinanceDbContext(DbContextOptions<HomeFinanceDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(HomeFinanceDbContext).Assembly);
    }
}
