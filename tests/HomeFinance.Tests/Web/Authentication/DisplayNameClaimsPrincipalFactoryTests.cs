using System.Security.Claims;
using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Entities;
using HomeFinance.Data;
using HomeFinance.Tests.Infrastructure;
using HomeFinance.Web.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HomeFinance.Tests.Web.Authentication;

public sealed class DisplayNameClaimsPrincipalFactoryTests : IDisposable
{
    private readonly TestDb _db;
    private readonly IServiceProvider _services;

    public DisplayNameClaimsPrincipalFactoryTests()
    {
        _db = new TestDb();

        var sc = new ServiceCollection();
        sc.AddLogging();
        sc.AddSingleton<HomeFinanceDbContext>(_db.Context);

        sc
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireLowercase = false;
                opts.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<HomeFinanceDbContext>()
            // AddDefaultTokenProviders() is intentionally omitted: it requires
            // IDataProtectionProvider which is only available in a full ASP.NET
            // Core host.  CreateAsync and CreateAsync(ClaimsPrincipal) work without it.
            .AddClaimsPrincipalFactory<DisplayNameClaimsPrincipalFactory>();

        _services = sc.BuildServiceProvider();
    }

    public void Dispose() => _db.Dispose();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<ApplicationUser> CreatePersistedUserAsync(
        string userName = "carol",
        string displayName = "Carol")
    {
        var userManager = _services.GetRequiredService<UserManager<ApplicationUser>>();
        var user = ApplicationUser.Create(new ApplicationUserData
        {
            UserName = userName,
            DisplayName = displayName,
        });
        var result = await userManager.CreateAsync(user, "Password1");
        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(e => e.Description)));
        return user;
    }

    // -------------------------------------------------------------------------
    // GenerateClaimsAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateClaimsAsync_UserWithDisplayName_AddsDisplayNameClaim()
    {
        var user = await CreatePersistedUserAsync(displayName: "Carol");
        var factory = _services.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();

        var principal = await factory.CreateAsync(user);

        var claim = principal.FindFirst("DisplayName");
        Assert.NotNull(claim);
        Assert.Equal("Carol", claim.Value);
    }

    [Fact]
    public async Task GenerateClaimsAsync_PreservesBaseClaimsAndAddsDisplayName()
    {
        var user = await CreatePersistedUserAsync(userName: "dave", displayName: "Dave");
        var factory = _services.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();

        var principal = await factory.CreateAsync(user);

        // Base factory always adds a NameIdentifier claim for the user id.
        Assert.NotNull(principal.FindFirst(ClaimTypes.NameIdentifier));

        // Our factory must additionally have the DisplayName claim.
        Assert.NotNull(principal.FindFirst("DisplayName"));
    }
}
