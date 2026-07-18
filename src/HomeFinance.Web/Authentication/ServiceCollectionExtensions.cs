using HomeFinance.Core.Entities;
using HomeFinance.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HomeFinance.Web.Authentication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentityCore<ApplicationUser>(opts =>
            {
                opts.SignIn.RequireConfirmedAccount = false;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opts.User.RequireUniqueEmail = false;
            })
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddEntityFrameworkStores<HomeFinanceDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<DisplayNameClaimsPrincipalFactory>();

        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.LoginPath = "/login";
                o.LogoutPath = "/logout";
                o.AccessDeniedPath = "/login";
                o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                o.SlidingExpiration = true;
            });

        services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services
            .Configure<SeededUsersOptions>(configuration.GetSection(SeededUsersOptions.SectionName))
            .PostConfigure<SeededUsersOptions>(o =>
            {
                if (o.Users.Count < 2)
                    throw new InvalidOperationException(
                        $"Configuration '{SeededUsersOptions.SectionName}' must contain at least two users.");
            });

        services.AddScoped<IUserSeeder, UserSeeder>();
        services.AddCascadingAuthenticationState();

        return services;
    }
}
