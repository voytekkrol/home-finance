using HomeFinance.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;

namespace HomeFinance.Core.Entities;

public sealed class ApplicationUser : IdentityUser
{
    // EF Core / Identity hydration only
    private ApplicationUser() { }

    public string DisplayName { get; private set; } = string.Empty;

    public static ApplicationUser Create(CreateApplicationUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DisplayName);

        var displayName = request.DisplayName.Trim();
        if (displayName.Length > 64)
            throw new ArgumentException("DisplayName must be 64 characters or fewer.", nameof(request));

        return new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            DisplayName = displayName,
        };
    }

    public void Rename(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        displayName = displayName.Trim();
        if (displayName.Length > 64)
            throw new ArgumentException("DisplayName must be 64 characters or fewer.", nameof(displayName));

        DisplayName = displayName;
    }
}
