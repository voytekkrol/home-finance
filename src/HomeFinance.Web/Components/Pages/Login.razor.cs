using Microsoft.AspNetCore.Components;

namespace HomeFinance.Web.Components.Pages;

public abstract class LoginBase : ComponentBase
{
    [SupplyParameterFromQuery(Name = "error")]
    public string? Error { get; set; }

    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery(Name = "userName")]
    public string? PrefilledUserName { get; set; }

    public LoginInputModel Input { get; } = new();

    public string? ErrorMessage => Error switch
    {
        "invalid" => "Invalid username or password.",
        "locked" => "Account is temporarily locked. Try again later.",
        _ => null,
    };

    // InputAttributes lets us inject the HTML name attribute that MudBlazor's analyzer
    // would reject if passed as a direct Razor attribute.
    protected readonly Dictionary<string, object> _userNameAttrs = new()
    {
        ["name"] = "userName",
        ["autocomplete"] = "username",
    };

    protected readonly Dictionary<string, object> _passwordAttrs = new()
    {
        ["name"] = "password",
        ["autocomplete"] = "current-password",
    };

    public sealed record LoginInputModel
    {
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
