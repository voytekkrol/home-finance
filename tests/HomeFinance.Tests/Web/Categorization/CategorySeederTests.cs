using HomeFinance.Core.Contracts.Categories;
using HomeFinance.Core.Entities;
using HomeFinance.Tests.Infrastructure;
using HomeFinance.Web.Categorization;

namespace HomeFinance.Tests.Web.Categorization;

public sealed class CategorySeederTests : IDisposable
{
    private readonly TestDb _db;

    public CategorySeederTests()
    {
        _db = new TestDb();
    }

    public void Dispose() => _db.Dispose();

    // -------------------------------------------------------------------------
    // SeedAsync — happy path: empty database
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_EmptyDatabase_InsertsAll39Categories()
    {
        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(39, _db.Context.Categories.Count());
    }

    // -------------------------------------------------------------------------
    // SeedAsync — idempotency
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_CalledTwice_IsIdempotent()
    {
        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);
        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(39, _db.Context.Categories.Count());
    }

    // -------------------------------------------------------------------------
    // SeedAsync — skips when data already present
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_DatabaseAlreadyHasOneCategory_InsertsNothing()
    {
        var existing = Category.Create(new CategoryData { Name = "Pre-existing" });
        _db.Context.Categories.Add(existing);
        await _db.Context.SaveChangesAsync();

        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(1, _db.Context.Categories.Count());

        var stored = _db.Context.Categories.Single();
        Assert.Equal(existing.Id, stored.Id);
        Assert.Equal("Pre-existing", stored.Name);
    }

    // -------------------------------------------------------------------------
    // SeedAsync — entity construction is done through Category.Create
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_EmptyDatabase_SetsNonDefaultCreatedUtc()
    {
        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);

        var categories = _db.Context.Categories.ToList();
        Assert.All(categories, c => Assert.NotEqual(default, c.CreatedUtc));
    }

    [Fact]
    public async Task SeedAsync_EmptyDatabase_LeavesIsArchivedFalseAndIconNull()
    {
        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);

        var categories = _db.Context.Categories.ToList();
        Assert.All(categories, c =>
        {
            Assert.False(c.IsArchived);
            Assert.Null(c.Icon);
        });
    }

    // -------------------------------------------------------------------------
    // SeedAsync — correct names and colors persisted
    // -------------------------------------------------------------------------

    public static TheoryData<string, string> ExpectedNamesAndColors()
    {
        var data = new TheoryData<string, string>
        {
            { "Business Revenue",           "#2E7D32" },
            { "Salary",                     "#388E3C" },
            { "Investment Income",          "#66BB6A" },
            { "Refunds & Reimbursements",   "#81C784" },
            { "Gifts Received",             "#A5D6A7" },
            { "Mortgage",                   "#1565C0" },
            { "Building & Community Fees",  "#1976D2" },
            { "Property & Home Insurance",  "#1E88E5" },
            { "Utilities",                  "#42A5F5" },
            { "Internet & Phone",           "#90CAF9" },
            { "Home Maintenance",           "#BBDEFB" },
            { "Groceries",                  "#E65100" },
            { "Dining Out",                 "#FF8F00" },
            { "Fuel",                       "#4527A0" },
            { "Vehicle Insurance",          "#7B1FA2" },
            { "Vehicle Maintenance",        "#AB47BC" },
            { "Public Transport",           "#CE93D8" },
            { "Healthcare",                 "#C62828" },
            { "Health Insurance",           "#EF5350" },
            { "Childcare & Education",      "#006064" },
            { "Children's Activities",      "#00838F" },
            { "Children's Needs",           "#00BCD4" },
            { "Personal Care",              "#AD1457" },
            { "Clothing & Footwear",        "#F06292" },
            { "Entertainment",              "#4A148C" },
            { "Subscriptions",              "#6A1B9A" },
            { "Travel & Holidays",          "#9C27B0" },
            { "Gifts Given",                "#F57F17" },
            { "Charitable Donations",       "#FF8F00" },
            { "Move to Savings Account",    "#00695C" },
            { "Investments",                "#00897B" },
            { "ZUS",                        "#BF360C" },
            { "Income Tax",                 "#D84315" },
            { "Accountant",                 "#FF7043" },
            { "Business Equipment",         "#37474F" },
            { "Business Software",          "#455A64" },
            { "Business Services",          "#607D8B" },
            { "Business Travel",            "#78909C" },
            { "Bank Fees",                  "#757575" },
        };
        return data;
    }

    [Theory]
    [MemberData(nameof(ExpectedNamesAndColors))]
    public async Task SeedAsync_EmptyDatabase_InsertsCorrectNamesAndColors(
        string expectedName,
        string expectedColor)
    {
        var seeder = new CategorySeeder(_db.Context);

        await seeder.SeedAsync(CancellationToken.None);

        var categories = _db.Context.Categories.ToList();
        Assert.Single(categories, c => c.Name == expectedName && c.ColorHex == expectedColor);
    }
}
