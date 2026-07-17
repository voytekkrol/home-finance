using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Validation;

namespace HomeFinance.Tests.Core.Entities;

public sealed class ApplicationUserTests
{
    // -------------------------------------------------------------------------
    // Create — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_ValidData_ReturnsUserWithDisplayName()
    {
        var user = ApplicationUser.Create(ValidData());

        Assert.Equal("Alice", user.DisplayName);
    }

    [Fact]
    public void Create_ValidData_SetsUserName()
    {
        var user = ApplicationUser.Create(ValidData());

        Assert.Equal("alice", user.UserName);
    }

    [Fact]
    public void Create_DisplayNameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "alice",
            DisplayName = "  Alice  ",
        });

        Assert.Equal("Alice", user.DisplayName);
    }

    [Fact]
    public void Create_UserNameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "  alice  ",
            DisplayName = "Alice",
        });

        Assert.Equal("alice", user.UserName);
    }

    // -------------------------------------------------------------------------
    // Create — null data
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ApplicationUser.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — UserName validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_UserNameIsNull_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() => ApplicationUser.Create(new ApplicationUserData
        {
            UserName = null!,
            DisplayName = "Alice",
        }));

        Assert.Equal(nameof(ApplicationUserData.UserName), ex.ParamName);
    }

    [Fact]
    public void Create_UserNameIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() => ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "   ",
            DisplayName = "Alice",
        }));

        Assert.Equal(nameof(ApplicationUserData.UserName), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — DisplayName validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_DisplayNameIsNull_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() => ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "alice",
            DisplayName = null!,
        }));

        Assert.Equal(nameof(ApplicationUserData.DisplayName), ex.ParamName);
    }

    [Fact]
    public void Create_DisplayNameIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var ex = Assert.Throws<MissingRequiredValueException>(() => ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "alice",
            DisplayName = "   ",
        }));

        Assert.Equal(nameof(ApplicationUserData.DisplayName), ex.ParamName);
    }

    [Fact]
    public void Create_DisplayNameExceeds64Chars_ThrowsLabelTooLongException()
    {
        var ex = Assert.Throws<LabelTooLongException>(() => ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "alice",
            DisplayName = new string('A', 65),
        }));

        Assert.Equal(nameof(ApplicationUserData.DisplayName), ex.ParamName);
    }

    [Fact]
    public void Create_DisplayNameExactly64Chars_Succeeds()
    {
        var user = ApplicationUser.Create(new ApplicationUserData
        {
            UserName = "alice",
            DisplayName = new string('A', 64),
        });

        Assert.Equal(64, user.DisplayName.Length);
    }

    // -------------------------------------------------------------------------
    // Rename
    // -------------------------------------------------------------------------

    [Fact]
    public void Rename_ValidName_ChangesDisplayName()
    {
        var user = ApplicationUser.Create(ValidData());

        user.Rename("Bob");

        Assert.Equal("Bob", user.DisplayName);
    }

    [Fact]
    public void Rename_NameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(ValidData());

        user.Rename("  Bob  ");

        Assert.Equal("Bob", user.DisplayName);
    }

    [Fact]
    public void Rename_NullName_ThrowsMissingRequiredValueException()
    {
        var user = ApplicationUser.Create(ValidData());

        var ex = Assert.Throws<MissingRequiredValueException>(() => user.Rename(null!));

        Assert.Equal("DisplayName", ex.ParamName);
    }

    [Fact]
    public void Rename_WhiteSpaceName_ThrowsMissingRequiredValueExceptionAndLeavesDisplayNameUnchanged()
    {
        var user = ApplicationUser.Create(ValidData());
        var original = user.DisplayName;

        var ex = Assert.Throws<MissingRequiredValueException>(() => user.Rename("   "));

        Assert.Equal(original, user.DisplayName);
        Assert.Equal("DisplayName", ex.ParamName);
    }

    [Fact]
    public void Rename_NameExceeds64Chars_ThrowsLabelTooLongExceptionAndLeavesDisplayNameUnchanged()
    {
        var user = ApplicationUser.Create(ValidData());
        var original = user.DisplayName;

        var ex = Assert.Throws<LabelTooLongException>(() => user.Rename(new string('B', 65)));

        Assert.Equal(original, user.DisplayName);
        Assert.Equal("DisplayName", ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static ApplicationUserData ValidData() =>
        new()
        {
            UserName = "alice",
            DisplayName = "Alice",
        };
}
