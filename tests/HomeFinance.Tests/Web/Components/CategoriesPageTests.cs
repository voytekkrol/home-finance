// bUnit ignores @rendermode InteractiveServer and renders in-process — no special hosting setup needed.
// No AuthenticationStateProvider required: Categories.razor has no [Authorize] and does not inject auth state.

using Bunit;
using HomeFinance.Core.Contracts.Categories;
using HomeFinance.Core.Entities;
using HomeFinance.Data;
using HomeFinance.Tests.Infrastructure;
using HomeFinance.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace HomeFinance.Tests.Web.Components;

public sealed class CategoriesPageTests : IDisposable
{
    private readonly TestDb _db;
    private readonly Bunit.TestContext _ctx;

    public CategoriesPageTests()
    {
        _db = new TestDb();

        _ctx = new Bunit.TestContext();
        _ctx.Services.AddSingleton(_db.Context);
        _ctx.Services.AddMudServices();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _db.Dispose();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static void SeedCategories(HomeFinanceDbContext ctx)
    {
        var seeder = new HomeFinance.Web.Categorization.CategorySeeder(ctx);
        seeder.SeedAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    // -------------------------------------------------------------------------
    // Renders correct number of rows
    // -------------------------------------------------------------------------

    [Fact]
    public void Categories_WithSeededData_Renders39TableRows()
    {
        SeedCategories(_db.Context);

        var cut = _ctx.RenderComponent<Categories>();

        // Each row is rendered as a <tr> inside the MudTable body.
        // MudTable wraps row content in <tr> elements; we count them excluding
        // the header row (which lives in a separate <thead>).
        var bodyRows = cut.FindAll("tbody tr");

        Assert.Equal(39, bodyRows.Count);
    }

    // -------------------------------------------------------------------------
    // Rows are ordered alphabetically by Name
    // -------------------------------------------------------------------------

    [Fact]
    public void Categories_RowsAreOrderedAlphabeticallyByName()
    {
        SeedCategories(_db.Context);

        var cut = _ctx.RenderComponent<Categories>();

        // Collect all <td> elements that hold the Name text (second <td> per row).
        var rows = cut.FindAll("tbody tr");
        var names = rows
            .Select(row => row.QuerySelectorAll("td").Skip(1).FirstOrDefault()?.TextContent.Trim())
            .Where(n => n is not null)
            .Select(n => n!)
            .ToList();

        var sorted = names.OrderBy(n => n, StringComparer.Ordinal).ToList();

        Assert.Equal(sorted, names);
    }

    // -------------------------------------------------------------------------
    // Each row has a color swatch
    // -------------------------------------------------------------------------

    [Fact]
    public void Categories_EachRowHasColorSwatch()
    {
        SeedCategories(_db.Context);

        var cut = _ctx.RenderComponent<Categories>();

        // The swatch is a <div class="category-swatch"> inside the first <td> of each row.
        var swatches = cut.FindAll("div.category-swatch");

        Assert.Equal(39, swatches.Count);
    }
}
