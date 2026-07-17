using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Entities;

namespace HomeFinance.Tests.Core.Entities;

public sealed class ApplicationUserTests
{
    // -------------------------------------------------------------------------
    // Create — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_ValidRequest_ReturnsUserWithDisplayName()
    {
        var user = ApplicationUser.Create(ValidRequest());

        Assert.Equal("Alice", user.DisplayName);
    }

    [Fact]
    public void Create_ValidRequest_SetsUserName()
    {
        var user = ApplicationUser.Create(ValidRequest());

        Assert.Equal("alice", user.UserName);
    }

    [Fact]
    public void Create_DisplayNameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "alice",
            DisplayName = "  Alice  ",
        });

        Assert.Equal("Alice", user.DisplayName);
    }

    [Fact]
    public void Create_UserNameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "  alice  ",
            DisplayName = "Alice",
        });

        Assert.Equal("alice", user.UserName);
    }

    // -------------------------------------------------------------------------
    // Create — null request
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ApplicationUser.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — UserName validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_UserNameIsNull_ThrowsArgumentException()
    {
        Assert.ThrowsAny<ArgumentException>(() => ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = null!,
            DisplayName = "Alice",
        }));
    }

    [Fact]
    public void Create_UserNameIsWhiteSpace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "   ",
            DisplayName = "Alice",
        }));
    }

    // -------------------------------------------------------------------------
    // Create — DisplayName validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_DisplayNameIsNull_ThrowsArgumentException()
    {
        Assert.ThrowsAny<ArgumentException>(() => ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "alice",
            DisplayName = null!,
        }));
    }

    [Fact]
    public void Create_DisplayNameIsWhiteSpace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "alice",
            DisplayName = "   ",
        }));
    }

    [Fact]
    public void Create_DisplayNameExceeds64Chars_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = "alice",
            DisplayName = new string('A', 65),
        }));
    }

    [Fact]
    public void Create_DisplayNameExactly64Chars_Succeeds()
    {
        var user = ApplicationUser.Create(new CreateApplicationUserRequest
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
        var user = ApplicationUser.Create(ValidRequest());

        user.Rename("Bob");

        Assert.Equal("Bob", user.DisplayName);
    }

    [Fact]
    public void Rename_NameWithSurroundingWhitespace_IsTrimmed()
    {
        var user = ApplicationUser.Create(ValidRequest());

        user.Rename("  Bob  ");

        Assert.Equal("Bob", user.DisplayName);
    }

    [Fact]
    public void Rename_NullName_ThrowsArgumentException()
    {
        var user = ApplicationUser.Create(ValidRequest());

        Assert.ThrowsAny<ArgumentException>(() => user.Rename(null!));
    }

    [Fact]
    public void Rename_WhiteSpaceName_ThrowsAndLeavesDisplayNameUnchanged()
    {
        var user = ApplicationUser.Create(ValidRequest());
        var original = user.DisplayName;

        Assert.Throws<ArgumentException>(() => user.Rename("   "));

        Assert.Equal(original, user.DisplayName);
    }

    [Fact]
    public void Rename_NameExceeds64Chars_ThrowsAndLeavesDisplayNameUnchanged()
    {
        var user = ApplicationUser.Create(ValidRequest());
        var original = user.DisplayName;

        Assert.Throws<ArgumentException>(() => user.Rename(new string('B', 65)));

        Assert.Equal(original, user.DisplayName);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static CreateApplicationUserRequest ValidRequest() =>
        new()
        {
            UserName = "alice",
            DisplayName = "Alice",
        };
}
