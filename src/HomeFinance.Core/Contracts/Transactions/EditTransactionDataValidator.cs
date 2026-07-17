using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Contracts.Transactions;

public static class EditTransactionDataValidator
{
    public static EditTransactionData Invoke(EditTransactionData data) => data with
    {
        OccurredOn = Rules.RequireNotFarFuture(data.OccurredOn, 1, nameof(data.OccurredOn)),
        Amount = Rules.RequireNonZero(data.Amount, nameof(data.Amount)),
        Description = Rules.RequireLabel(data.Description, maxLength: 256, nameof(data.Description)),
        AccountId = Rules.RequireNonEmptyGuid(data.AccountId, nameof(data.AccountId)),
        CategoryId = Rules.RequireNonEmptyGuid(data.CategoryId, nameof(data.CategoryId)),
    };
}
