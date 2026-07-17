using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Validation;
using Microsoft.AspNetCore.Identity;

namespace HomeFinance.Core.Entities;

public sealed class ApplicationUser : IdentityUser
{
    // EF Core / Identity hydration only
    private ApplicationUser() { }

    public string DisplayName { get; private set; } = string.Empty;

    public static ApplicationUser Create(ApplicationUserData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        data = ApplicationUserDataValidator.Invoke(data);
        return new ApplicationUser
        {
            UserName = data.UserName,
            DisplayName = data.DisplayName,
        };
    }

    public void Rename(string displayName)
    {
        DisplayName = Rules.RequireLabel(displayName, 64, nameof(DisplayName));
    }
}
