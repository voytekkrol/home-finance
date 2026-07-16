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

    public static Transaction Create(
        DateOnly occurredOn,
        decimal amount,
        string description,
        Guid accountId,
        Guid categoryId,
        string enteredByUserId)
    {
        if (occurredOn > DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
            throw new ArgumentOutOfRangeException(nameof(occurredOn), "OccurredOn cannot be more than one day in the future.");

        if (amount == 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-zero.");

        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        description = description.Trim();
        if (description.Length > 256)
            throw new ArgumentException("Description must be 256 characters or fewer.", nameof(description));

        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId is required.", nameof(accountId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));

        ArgumentException.ThrowIfNullOrWhiteSpace(enteredByUserId);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            OccurredOn = occurredOn,
            Amount = amount,
            Description = description,
            AccountId = accountId,
            CategoryId = categoryId,
            EnteredByUserId = enteredByUserId,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Edit(
        DateOnly occurredOn,
        decimal amount,
        string description,
        Guid accountId,
        Guid categoryId)
    {
        if (occurredOn > DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
            throw new ArgumentOutOfRangeException(nameof(occurredOn), "OccurredOn cannot be more than one day in the future.");

        if (amount == 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-zero.");

        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        description = description.Trim();
        if (description.Length > 256)
            throw new ArgumentException("Description must be 256 characters or fewer.", nameof(description));

        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId is required.", nameof(accountId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));

        OccurredOn = occurredOn;
        Amount = amount;
        Description = description;
        AccountId = accountId;
        CategoryId = categoryId;
        UpdatedUtc = DateTime.UtcNow;
    }
}
