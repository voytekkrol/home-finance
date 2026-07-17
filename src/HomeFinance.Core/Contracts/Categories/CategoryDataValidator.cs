using HomeFinance.Core.Categorization;
using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Contracts.Categories;

public static class CategoryDataValidator
{
    public static CategoryData Invoke(CategoryData data) => data with
    {
        Name = Rules.RequireLabel(data.Name, maxLength: 64, nameof(data.Name)),
        ColorHex = string.IsNullOrWhiteSpace(data.ColorHex)
            ? Colors.DefaultCategory
            : Rules.RequireHexColor(data.ColorHex, nameof(data.ColorHex)),
        Icon = Rules.RequireOptionalLabel(data.Icon, maxLength: 128, nameof(data.Icon)),
    };
}
