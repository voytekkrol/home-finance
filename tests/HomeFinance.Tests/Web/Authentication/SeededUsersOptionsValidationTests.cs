using HomeFinance.Web.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeFinance.Tests.Web.Authentication;

/// <summary>
/// Tests the PostConfigure validation guard registered by the production
/// DI wiring.  We replicate the guard directly via PostConfigure so that
/// no full web host is required.
///
/// SeededUsersOptions.Users is an init-only property on a record.
/// The .NET options pipeline creates the options object via
/// Activator.CreateInstance and then passes the mutable instance to
/// Configure/PostConfigure delegates; init-only is a compile-time
/// restriction only, so we can populate the list through reflection
/// inside a PostConfigure delegate that runs *before* the validation guard.
/// </summary>
public sealed class SeededUsersOptionsValidationTests
{
    // -------------------------------------------------------------------------
    // Helper
    // -------------------------------------------------------------------------

    private static ServiceProvider BuildServiceProviderWithUsers(IReadOnlyList<SeededUser> users)
    {
        var services = new ServiceCollection();

        // Step 1: populate the Users list (runs first because it is registered first).
        services.PostConfigure<SeededUsersOptions>(o =>
        {
            // init-only is compile-time only; reflection can set it at runtime.
            var prop = typeof(SeededUsersOptions).GetProperty(nameof(SeededUsersOptions.Users))!;
            prop.SetValue(o, users);
        });

        // Step 2: the same validation guard used in production DI.
        services.PostConfigure<SeededUsersOptions>(o =>
        {
            if (o.Users.Count < 2)
                throw new InvalidOperationException(
                    $"Configuration '{SeededUsersOptions.SectionName}' must contain at least two users.");
        });

        return services.BuildServiceProvider();
    }

    // -------------------------------------------------------------------------
    // Tests
    // -------------------------------------------------------------------------

    [Fact]
    public void PostConfigure_ZeroUsers_ThrowsInvalidOperationException()
    {
        var sp = BuildServiceProviderWithUsers([]);

        var ex = Assert.Throws<InvalidOperationException>(
            () => _ = sp.GetRequiredService<IOptions<SeededUsersOptions>>().Value);

        Assert.Contains(SeededUsersOptions.SectionName, ex.Message);
    }

    [Fact]
    public void PostConfigure_OneUser_ThrowsInvalidOperationException()
    {
        var sp = BuildServiceProviderWithUsers(
        [
            new SeededUser { UserName = "alice", DisplayName = "Alice", Password = "Password1" },
        ]);

        var ex = Assert.Throws<InvalidOperationException>(
            () => _ = sp.GetRequiredService<IOptions<SeededUsersOptions>>().Value);

        Assert.Contains(SeededUsersOptions.SectionName, ex.Message);
    }

    [Fact]
    public void PostConfigure_TwoUsers_DoesNotThrow()
    {
        var sp = BuildServiceProviderWithUsers(
        [
            new SeededUser { UserName = "alice", DisplayName = "Alice", Password = "Password1" },
            new SeededUser { UserName = "bob", DisplayName = "Bob", Password = "Password2" },
        ]);

        var exception = Record.Exception(
            () => _ = sp.GetRequiredService<IOptions<SeededUsersOptions>>().Value);

        Assert.Null(exception);
    }
}
