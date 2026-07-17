using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Contracts.Accounts;

public static class AccountDataValidator
{
    public static AccountData Invoke(AccountData data) => data with
    {
        Name = Rules.RequireLabel(data.Name, maxLength: 64, nameof(data.Name)),
        OwnerUserId = Rules.RequireIdentityUserId(data.OwnerUserId, nameof(data.OwnerUserId)),
        Type = Rules.RequireDefined(data.Type, nameof(data.Type)),
        Currency = Rules.RequireIsoCurrencyCode(data.Currency, nameof(data.Currency)),
    };
}
