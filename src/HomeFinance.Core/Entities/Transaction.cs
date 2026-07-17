using HomeFinance.Core.Contracts.Transactions;

namespace HomeFinance.Core.Entities;

public sealed class Transaction
{
    // EF Core hydration only
    private Transaction() { }

    public Guid Id { get; init; }
    public DateOnly OccurredOn { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid AccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string EnteredByUserId { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; private set; }

    // EF populates these navigations; they are never read before hydration
    public Account Account { get; private set; } = null!;
    public Category Category { get; private set; } = null!;

    public static Transaction Create(CreateTransactionData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        data = CreateTransactionDataValidator.Invoke(data);
        return new Transaction
        {
            Id = Guid.NewGuid(),
            OccurredOn = data.OccurredOn,
            Amount = data.Amount,
            Description = data.Description,
            AccountId = data.AccountId,
            CategoryId = data.CategoryId,
            EnteredByUserId = data.EnteredByUserId,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Edit(EditTransactionData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        data = EditTransactionDataValidator.Invoke(data);

        OccurredOn = data.OccurredOn;
        Amount = data.Amount;
        Description = data.Description;
        AccountId = data.AccountId;
        CategoryId = data.CategoryId;
        UpdatedUtc = DateTime.UtcNow;
    }
}
