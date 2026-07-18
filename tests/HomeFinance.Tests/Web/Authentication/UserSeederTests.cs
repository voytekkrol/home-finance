using HomeFinance.Core.Entities;
using HomeFinance.Data;
using HomeFinance.Tests.Infrastructure;
using HomeFinance.Web.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeFinance.Tests.Web.Authentication;

public sealed class UserSeederTests : IDisposable
{
    private readonly TestDb _db;

    public UserSeederTests()
    {
        _db = new TestDb();
    }

    public void Dispose() => _db.Dispose();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Builds a <see cref="UserManager{TUser}"/> backed by the shared in-memory
    /// SQLite connection, with the same password policy the production app uses.
    /// </summary>
    private UserManager<ApplicationUser> BuildUserManager(int requiredPasswordLength = 8)
    {
        var services = new ServiceCollection();

        services.AddLogging();

        // Supply the already-migrated context from TestDb so we share the
        // same in-memory SQLite connection across the test.
        services.AddSingleton<HomeFinanceDbContext>(_db.Context);

        services
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.Password.RequiredLength = requiredPasswordLength;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireLowercase = false;
                opts.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<HomeFinanceDbContext>();
        // AddDefaultTokenProviders() is intentionally omitted: it requires
        // IDataProtectionProvider which is only available in a full ASP.NET Core
        // host.  UserManager.CreateAsync and FindByNameAsync work without it.

        return services.BuildServiceProvider().GetRequiredService<UserManager<ApplicationUser>>();
    }

    private static IOptions<SeededUsersOptions> OptionsFor(params SeededUser[] users)
    {
        var opts = new SeededUsersOptions { Users = users };
        return Microsoft.Extensions.Options.Options.Create(opts);
    }

    private static SeededUser MakeSeededUser(string userName = "alice", string displayName = "Alice", string password = "Password1") =>
        new SeededUser { UserName = userName, DisplayName = displayName, Password = password };

    // -------------------------------------------------------------------------
    // SeedAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_EmptyDatabase_CreatesBothUsers()
    {
        var userManager = BuildUserManager();
        var options = OptionsFor(
            MakeSeededUser("alice", "Alice"),
            MakeSeededUser("bob", "Bob"));
        var seeder = new UserSeeder(userManager, options);

        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(2, _db.Context.Users.Count());
    }

    [Fact]
    public async Task SeedAsync_SetsDisplayNameFromConfig()
    {
        var userManager = BuildUserManager();
        var options = OptionsFor(
            MakeSeededUser("alice", "Alice"),
            MakeSeededUser("bob", "Bob"));
        var seeder = new UserSeeder(userManager, options);

        await seeder.SeedAsync(CancellationToken.None);

        var alice = await userManager.FindByNameAsync("alice");
        var bob = await userManager.FindByNameAsync("bob");
        Assert.NotNull(alice);
        Assert.NotNull(bob);
        Assert.Equal("Alice", alice.DisplayName);
        Assert.Equal("Bob", bob.DisplayName);
    }

    [Fact]
    public async Task SeedAsync_SetsEmailConfirmedTrue()
    {
        var userManager = BuildUserManager();
        var options = OptionsFor(
            MakeSeededUser("alice", "Alice"),
            MakeSeededUser("bob", "Bob"));
        var seeder = new UserSeeder(userManager, options);

        await seeder.SeedAsync(CancellationToken.None);

        var alice = await userManager.FindByNameAsync("alice");
        var bob = await userManager.FindByNameAsync("bob");
        Assert.NotNull(alice);
        Assert.NotNull(bob);
        Assert.True(alice.EmailConfirmed);
        Assert.True(bob.EmailConfirmed);
    }

    [Fact]
    public async Task SeedAsync_SetsEmailToUserNameAtDomain()
    {
        var userManager = BuildUserManager();
        var options = OptionsFor(
            MakeSeededUser("alice", "Alice"),
            MakeSeededUser("bob", "Bob"));
        var seeder = new UserSeeder(userManager, options);

        await seeder.SeedAsync(CancellationToken.None);

        var alice = await userManager.FindByNameAsync("alice");
        var bob = await userManager.FindByNameAsync("bob");
        Assert.NotNull(alice);
        Assert.NotNull(bob);
        Assert.Equal("alice@homefinance.local", alice.Email);
        Assert.Equal("bob@homefinance.local", bob.Email);
    }

    // -------------------------------------------------------------------------
    // SeedAsync — idempotency
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_CalledTwice_IsIdempotent()
    {
        var userManager = BuildUserManager();
        var options = OptionsFor(
            MakeSeededUser("alice", "Alice"),
            MakeSeededUser("bob", "Bob"));
        var seeder = new UserSeeder(userManager, options);

        await seeder.SeedAsync(CancellationToken.None);
        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(2, _db.Context.Users.Count());
    }

    // -------------------------------------------------------------------------
    // SeedAsync — failure path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SeedAsync_UserManagerCreateFails_ThrowsInvalidOperationExceptionContainingUserName()
    {
        // RequiredLength=8 but we pass a 3-char password — CreateAsync returns errors.
        var userManager = BuildUserManager(requiredPasswordLength: 8);
        var options = OptionsFor(
            new SeededUser { UserName = "alice", DisplayName = "Alice", Password = "abc" });
        var seeder = new UserSeeder(userManager, options);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => seeder.SeedAsync(CancellationToken.None));

        Assert.Contains("alice", ex.Message);
    }
}
