using HomeFinance.Core.Contracts.Categories;

namespace HomeFinance.Web.Categorization;

public static class CategorySeedData
{
    public static IReadOnlyList<CategoryData> All { get; } =
    [
        new CategoryData { Name = "Business Revenue",          ColorHex = "#2E7D32" },
        new CategoryData { Name = "Salary",                    ColorHex = "#388E3C" },
        new CategoryData { Name = "Investment Income",         ColorHex = "#66BB6A" },
        new CategoryData { Name = "Refunds & Reimbursements",  ColorHex = "#81C784" },
        new CategoryData { Name = "Gifts Received",            ColorHex = "#A5D6A7" },
        new CategoryData { Name = "Mortgage",                  ColorHex = "#1565C0" },
        new CategoryData { Name = "Building & Community Fees", ColorHex = "#1976D2" },
        new CategoryData { Name = "Property & Home Insurance", ColorHex = "#1E88E5" },
        new CategoryData { Name = "Utilities",                 ColorHex = "#42A5F5" },
        new CategoryData { Name = "Internet & Phone",          ColorHex = "#90CAF9" },
        new CategoryData { Name = "Home Maintenance",          ColorHex = "#BBDEFB" },
        new CategoryData { Name = "Groceries",                 ColorHex = "#E65100" },
        new CategoryData { Name = "Dining Out",                ColorHex = "#FF8F00" },
        new CategoryData { Name = "Fuel",                      ColorHex = "#4527A0" },
        new CategoryData { Name = "Vehicle Insurance",         ColorHex = "#7B1FA2" },
        new CategoryData { Name = "Vehicle Maintenance",       ColorHex = "#AB47BC" },
        new CategoryData { Name = "Public Transport",          ColorHex = "#CE93D8" },
        new CategoryData { Name = "Healthcare",                ColorHex = "#C62828" },
        new CategoryData { Name = "Health Insurance",          ColorHex = "#EF5350" },
        new CategoryData { Name = "Childcare & Education",     ColorHex = "#006064" },
        new CategoryData { Name = "Children's Activities",     ColorHex = "#00838F" },
        new CategoryData { Name = "Children's Needs",          ColorHex = "#00BCD4" },
        new CategoryData { Name = "Personal Care",             ColorHex = "#AD1457" },
        new CategoryData { Name = "Clothing & Footwear",       ColorHex = "#F06292" },
        new CategoryData { Name = "Entertainment",             ColorHex = "#4A148C" },
        new CategoryData { Name = "Subscriptions",             ColorHex = "#6A1B9A" },
        new CategoryData { Name = "Travel & Holidays",         ColorHex = "#9C27B0" },
        new CategoryData { Name = "Gifts Given",               ColorHex = "#F57F17" },
        new CategoryData { Name = "Charitable Donations",      ColorHex = "#FF8F00" },
        new CategoryData { Name = "Move to Savings Account",   ColorHex = "#00695C" },
        new CategoryData { Name = "Investments",               ColorHex = "#00897B" },
        new CategoryData { Name = "ZUS",                       ColorHex = "#BF360C" },
        new CategoryData { Name = "Income Tax",                ColorHex = "#D84315" },
        new CategoryData { Name = "Accountant",                ColorHex = "#FF7043" },
        new CategoryData { Name = "Business Equipment",        ColorHex = "#37474F" },
        new CategoryData { Name = "Business Software",         ColorHex = "#455A64" },
        new CategoryData { Name = "Business Services",         ColorHex = "#607D8B" },
        new CategoryData { Name = "Business Travel",           ColorHex = "#78909C" },
        new CategoryData { Name = "Bank Fees",                 ColorHex = "#757575" },
    ];
}
