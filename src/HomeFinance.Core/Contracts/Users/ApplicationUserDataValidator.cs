using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Contracts.Users;

public static class ApplicationUserDataValidator
{
    public static ApplicationUserData Invoke(ApplicationUserData data) => data with
    {
        UserName = Rules.RequireLabel(data.UserName, 256, nameof(data.UserName)),
        DisplayName = Rules.RequireLabel(data.DisplayName, 64, nameof(data.DisplayName)),
    };
}
