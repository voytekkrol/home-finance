using System.Text.RegularExpressions;
using HomeFinance.Web.Categorization;

namespace HomeFinance.Tests.Web.Categorization;

public sealed class CategorySeedDataTests
{
    // -------------------------------------------------------------------------
    // Count
    // -------------------------------------------------------------------------

    [Fact]
    public void All_ContainsExactly39Entries()
    {
        Assert.Equal(39, CategorySeedData.All.Count);
    }

    // -------------------------------------------------------------------------
    // Uniqueness
    // -------------------------------------------------------------------------

    [Fact]
    public void All_HasUniqueNames()
    {
        var duplicates = CategorySeedData.All
            .GroupBy(d => d.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.Empty(duplicates);
    }

    // -------------------------------------------------------------------------
    // Color format
    // -------------------------------------------------------------------------

    [Fact]
    public void All_AllColorsAreValidHexFormat()
    {
        var hexPattern = new Regex(@"^#[0-9A-Fa-f]{6}$");

        Assert.All(CategorySeedData.All, entry =>
        {
            Assert.NotNull(entry.ColorHex);
            Assert.Matches(hexPattern, entry.ColorHex);
        });
    }

    // -------------------------------------------------------------------------
    // Icon
    // -------------------------------------------------------------------------

    [Fact]
    public void All_IconIsNullForAllEntries()
    {
        Assert.All(CategorySeedData.All, entry => Assert.Null(entry.Icon));
    }

    // -------------------------------------------------------------------------
    // Exact seed list — order and values
    // -------------------------------------------------------------------------

    public static TheoryData<int, string, string> SeedEntries()
    {
        var data = new TheoryData<int, string, string>
        {
            { 0,  "Business Revenue",           "#2E7D32" },
            { 1,  "Salary",                     "#388E3C" },
            { 2,  "Investment Income",           "#66BB6A" },
            { 3,  "Refunds & Reimbursements",   "#81C784" },
            { 4,  "Gifts Received",             "#A5D6A7" },
            { 5,  "Mortgage",                   "#1565C0" },
            { 6,  "Building & Community Fees",  "#1976D2" },
            { 7,  "Property & Home Insurance",  "#1E88E5" },
            { 8,  "Utilities",                  "#42A5F5" },
            { 9,  "Internet & Phone",           "#90CAF9" },
            { 10, "Home Maintenance",           "#BBDEFB" },
            { 11, "Groceries",                  "#E65100" },
            { 12, "Dining Out",                 "#FF8F00" },
            { 13, "Fuel",                       "#4527A0" },
            { 14, "Vehicle Insurance",          "#7B1FA2" },
            { 15, "Vehicle Maintenance",        "#AB47BC" },
            { 16, "Public Transport",           "#CE93D8" },
            { 17, "Healthcare",                 "#C62828" },
            { 18, "Health Insurance",           "#EF5350" },
            { 19, "Childcare & Education",      "#006064" },
            { 20, "Children's Activities",      "#00838F" },
            { 21, "Children's Needs",           "#00BCD4" },
            { 22, "Personal Care",              "#AD1457" },
            { 23, "Clothing & Footwear",        "#F06292" },
            { 24, "Entertainment",              "#4A148C" },
            { 25, "Subscriptions",              "#6A1B9A" },
            { 26, "Travel & Holidays",          "#9C27B0" },
            { 27, "Gifts Given",                "#F57F17" },
            { 28, "Charitable Donations",       "#FF8F00" },
            { 29, "Move to Savings Account",    "#00695C" },
            { 30, "Investments",                "#00897B" },
            { 31, "ZUS",                        "#BF360C" },
            { 32, "Income Tax",                 "#D84315" },
            { 33, "Accountant",                 "#FF7043" },
            { 34, "Business Equipment",         "#37474F" },
            { 35, "Business Software",          "#455A64" },
            { 36, "Business Services",          "#607D8B" },
            { 37, "Business Travel",            "#78909C" },
            { 38, "Bank Fees",                  "#757575" },
        };
        return data;
    }

    [Theory]
    [MemberData(nameof(SeedEntries))]
    public void All_MatchesDecisionsSeedListInOrder(int index, string expectedName, string expectedColorHex)
    {
        var entry = CategorySeedData.All[index];

        Assert.Equal(expectedName, entry.Name);
        Assert.Equal(expectedColorHex, entry.ColorHex);
    }
}
