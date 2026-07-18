using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HomeFinance.Web.Authentication;

public sealed class UserSeeder : IUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SeededUsersOptions _options;

    public UserSeeder(UserManager<ApplicationUser> userManager, IOptions<SeededUsersOptions> options)
    {
        _userManager = userManager;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        foreach (var seededUser in _options.Users)
        {
            var existing = await _userManager.FindByNameAsync(seededUser.UserName);
            if (existing is not null)
                continue;

            var user = ApplicationUser.Create(new ApplicationUserData
            {
                UserName = seededUser.UserName,
                DisplayName = seededUser.DisplayName,
            });

            user.Email = seededUser.UserName + "@homefinance.local";
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, seededUser.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed user '{seededUser.UserName}': {errors}");
            }
        }
    }
}
