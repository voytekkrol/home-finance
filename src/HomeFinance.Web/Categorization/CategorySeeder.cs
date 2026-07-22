using HomeFinance.Core.Entities;
using HomeFinance.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeFinance.Web.Categorization;

internal sealed class CategorySeeder : ICategorySeeder
{
    private readonly HomeFinanceDbContext _db;

    public CategorySeeder(HomeFinanceDbContext db) => _db = db;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _db.Categories.AnyAsync(ct)) return;
        var categories = CategorySeedData.All
            .Select(Category.Create)
            .ToList();
        _db.Categories.AddRange(categories);
        await _db.SaveChangesAsync(ct);
    }
}
