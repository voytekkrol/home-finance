using HomeFinance.Web.Components.Pages;

namespace HomeFinance.Tests.Web.Authentication;

public sealed class LoginBaseTests
{
    // Minimal concrete subclass — adds nothing, just makes the abstract class instantiable.
    private sealed class ConcreteLogin : LoginBase { }

    // -------------------------------------------------------------------------
    // ErrorMessage property
    // -------------------------------------------------------------------------

    [Fact]
    public void ErrorMessage_WhenErrorIsInvalid_ReturnsInvalidCredentialsMessage()
    {
        var login = new ConcreteLogin { Error = "invalid" };

        Assert.Equal("Invalid username or password.", login.ErrorMessage);
    }

    [Fact]
    public void ErrorMessage_WhenErrorIsLocked_ReturnsLockedMessage()
    {
        var login = new ConcreteLogin { Error = "locked" };

        Assert.Equal("Account is temporarily locked. Try again later.", login.ErrorMessage);
    }

    [Fact]
    public void ErrorMessage_WhenErrorIsUnrecognized_ReturnsNull()
    {
        var login = new ConcreteLogin { Error = "other" };

        Assert.Null(login.ErrorMessage);
    }

    [Fact]
    public void ErrorMessage_WhenErrorIsNull_ReturnsNull()
    {
        // Error defaults to null — no assignment needed.
        var login = new ConcreteLogin();

        Assert.Null(login.ErrorMessage);
    }
}
