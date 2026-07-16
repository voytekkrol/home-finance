using Microsoft.AspNetCore.Identity;

namespace HomeFinance.Core.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
