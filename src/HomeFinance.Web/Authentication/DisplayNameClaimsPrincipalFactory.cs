using System.Security.Claims;
using HomeFinance.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HomeFinance.Web.Authentication;

public sealed class DisplayNameClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public DisplayNameClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("DisplayName", user.DisplayName));
        return identity;
    }
}
