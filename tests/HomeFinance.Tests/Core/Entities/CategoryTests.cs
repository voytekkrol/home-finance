using HomeFinance.Core.Categorization;
using HomeFinance.Core.Contracts.Categories;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Validation;

namespace HomeFinance.Tests.Core.Entities;

public sealed class CategoryTests
{
    // -------------------------------------------------------------------------
    // Create — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_MinimalData_UsesDefaultCategoryColor()
    {
        var category = Category.Create(new CategoryData { Name = "Groceries" });

        Assert.Equal(Colors.DefaultCategory, category.ColorHex);
    }

    [Fact]
    public void Create_MinimalData_IconIsNull()
    {
        var category = Category.Create(new CategoryData { Name = "Groceries" });

        Assert.Null(category.Icon);
    }

    [Fact]
    public void Create_ValidData_ReturnsWithGeneratedId()
    {
        var category = Category.Create(new CategoryData { Name = "Groceries" });

        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void Create_ValidData_SetsCreatedUtcToApproximatelyNow()
    {
        var before = DateTime.UtcNow;

        var category = Category.Create(new CategoryData { Name = "Groceries" });

        var after = DateTime.UtcNow;
        Assert.InRange(category.CreatedUtc, before, after);
    }

    // -------------------------------------------------------------------------
    // Create — null data
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Category.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — Name validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NameIsNull_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() =>
            Category.Create(new CategoryData { Name = null! }));

        Assert.Equal(nameof(CategoryData.Name), ex.ParamName);
    }

    [Fact]
    public void Create_NameIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() =>
            Category.Create(new CategoryData { Name = "   " }));

        Assert.Equal(nameof(CategoryData.Name), ex.ParamName);
    }

    [Fact]
    public void Create_NameExceeds64Chars_ThrowsLabelTooLongException()
    {
        var ex = Assert.Throws<LabelTooLongException>(() =>
            Category.Create(new CategoryData { Name = new string('X', 65) }));

        Assert.Equal(nameof(CategoryData.Name), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — ColorHex: default fallback
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_ColorHexIsNull_FallsBackToDefaultColor()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            ColorHex = null,
        });

        Assert.Equal(Colors.DefaultCategory, category.ColorHex);
    }

    [Fact]
    public void Create_ColorHexIsWhiteSpace_FallsBackToDefaultColor()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            ColorHex = "   ",
        });

        Assert.Equal(Colors.DefaultCategory, category.ColorHex);
    }

    [Fact]
    public void Create_EmptyStringColorHex_FallsBackToDefaultColor()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            ColorHex = "",
        });

        Assert.Equal(Colors.DefaultCategory, category.ColorHex);
    }

    // -------------------------------------------------------------------------
    // Create — ColorHex: invalid format
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("607D8B")]     // missing '#'
    [InlineData("#607D8")]     // too short
    [InlineData("#607D8BB")]   // too long
    [InlineData("#GGGGGG")]    // non-hex characters
    public void Create_MalformedColorHex_ThrowsInvalidHexColorException(string badColor)
    {
        var ex = Assert.Throws<InvalidHexColorException>(() =>
            Category.Create(new CategoryData
            {
                Name = "Food",
                ColorHex = badColor,
            }));

        Assert.Equal(nameof(CategoryData.ColorHex), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — ColorHex: normalization
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_LowercaseColorHex_NormalizesToUpperCase()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            ColorHex = "#607d8b",
        });

        Assert.Equal("#607D8B", category.ColorHex);
    }

    // -------------------------------------------------------------------------
    // Create — Icon validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_IconIsNull_StoredAsNull()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            Icon = null,
        });

        Assert.Null(category.Icon);
    }

    [Fact]
    public void Create_IconIsWhiteSpace_StoredAsNull()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            Icon = "   ",
        });

        Assert.Null(category.Icon);
    }

    [Fact]
    public void Create_IconExceeds128Chars_ThrowsLabelTooLongException()
    {
        var ex = Assert.Throws<LabelTooLongException>(() =>
            Category.Create(new CategoryData
            {
                Name = "Food",
                Icon = new string('i', 129),
            }));

        Assert.Equal(nameof(CategoryData.Icon), ex.ParamName);
    }

    [Fact]
    public void Create_ValidIconWithSurroundingWhitespace_StoredTrimmed()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            Icon = "  shopping_cart  ",
        });

        Assert.Equal("shopping_cart", category.Icon);
    }

    // -------------------------------------------------------------------------
    // Rename
    // -------------------------------------------------------------------------

    [Fact]
    public void Rename_ValidName_ChangesName()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.Rename("Dining");

        Assert.Equal("Dining", category.Name);
    }

    [Fact]
    public void Rename_WhiteSpaceName_ThrowsMissingRequiredValueExceptionAndLeavesNameUnchanged()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        var ex = Assert.Throws<MissingRequiredValueException>(() => category.Rename("   "));

        Assert.Equal("Food", category.Name);
        Assert.Equal("Name", ex.ParamName);
    }

    [Fact]
    public void Rename_NameExceeds64Chars_ThrowsLabelTooLongExceptionAndLeavesNameUnchanged()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        var ex = Assert.Throws<LabelTooLongException>(() => category.Rename(new string('Y', 65)));

        Assert.Equal("Food", category.Name);
        Assert.Equal("Name", ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // ChangeColor
    // -------------------------------------------------------------------------

    [Fact]
    public void ChangeColor_ValidColor_UpdatesColorHex()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.ChangeColor("#FF5733");

        Assert.Equal("#FF5733", category.ColorHex);
    }

    [Fact]
    public void ChangeColor_LowercaseColor_NormalizesToUpperCase()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.ChangeColor("#ff5733");

        Assert.Equal("#FF5733", category.ColorHex);
    }

    [Fact]
    public void ChangeColor_WhiteSpaceColor_ThrowsMissingRequiredValueExceptionAndLeavesColorUnchanged()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });
        var originalColor = category.ColorHex;

        var ex = Assert.Throws<MissingRequiredValueException>(() => category.ChangeColor("   "));

        Assert.Equal(originalColor, category.ColorHex);
        Assert.Equal("ColorHex", ex.ParamName);
    }

    [Fact]
    public void ChangeColor_MalformedColor_ThrowsInvalidHexColorExceptionAndLeavesColorUnchanged()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });
        var originalColor = category.ColorHex;

        var ex = Assert.Throws<InvalidHexColorException>(() => category.ChangeColor("607D8B"));

        Assert.Equal(originalColor, category.ColorHex);
        Assert.Equal("ColorHex", ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // ChangeIcon
    // -------------------------------------------------------------------------

    [Fact]
    public void ChangeIcon_NullValue_ClearsIcon()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            Icon = "shopping_cart",
        });

        category.ChangeIcon(null);

        Assert.Null(category.Icon);
    }

    [Fact]
    public void ChangeIcon_WhiteSpaceValue_ClearsIcon()
    {
        var category = Category.Create(new CategoryData
        {
            Name = "Food",
            Icon = "shopping_cart",
        });

        category.ChangeIcon("   ");

        Assert.Null(category.Icon);
    }

    [Fact]
    public void ChangeIcon_ValidValue_StoredTrimmed()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.ChangeIcon("  home  ");

        Assert.Equal("home", category.Icon);
    }

    [Fact]
    public void ChangeIcon_ValueExceeds128Chars_ThrowsLabelTooLongException()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        var ex = Assert.Throws<LabelTooLongException>(() => category.ChangeIcon(new string('i', 129)));

        Assert.Equal("Icon", ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Archive / Unarchive
    // -------------------------------------------------------------------------

    [Fact]
    public void Archive_WhenNotArchived_SetsIsArchivedTrue()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.Archive();

        Assert.True(category.IsArchived);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_IsNoOp()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });
        category.Archive();

        category.Archive();

        Assert.True(category.IsArchived);
    }

    [Fact]
    public void Unarchive_WhenArchived_SetsIsArchivedFalse()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });
        category.Archive();

        category.Unarchive();

        Assert.False(category.IsArchived);
    }

    [Fact]
    public void Unarchive_WhenNotArchived_IsNoOp()
    {
        var category = Category.Create(new CategoryData { Name = "Food" });

        category.Unarchive();

        Assert.False(category.IsArchived);
    }
}
