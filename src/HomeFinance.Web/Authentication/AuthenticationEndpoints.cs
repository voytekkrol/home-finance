using HomeFinance.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeFinance.Web.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/login", async (
            [FromForm] string? userName,
            [FromForm] string? password,
            [FromForm] string? returnUrl,
            SignInManager<ApplicationUser> signInManager) =>
        {
            var safeReturn = IsLocalUrl(returnUrl) ? returnUrl! : "/";

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return Results.Redirect(
                    $"/login?error=invalid&returnUrl={Uri.EscapeDataString(safeReturn)}");

            var result = await signInManager.PasswordSignInAsync(
                userName, password, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return Results.Redirect(
                    $"/login?error=locked&returnUrl={Uri.EscapeDataString(safeReturn)}");

            if (!result.Succeeded)
                return Results.Redirect(
                    $"/login?error=invalid&returnUrl={Uri.EscapeDataString(safeReturn)}");

            return Results.Redirect(safeReturn);
        }).DisableAntiforgery().AllowAnonymous();

        endpoints.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Redirect("/login");
        });

        return endpoints;
    }

    private static bool IsLocalUrl(string? url) =>
        !string.IsNullOrEmpty(url) &&
        url.StartsWith('/') &&
        !url.StartsWith("//") &&
        !url.StartsWith("/\\");
}
