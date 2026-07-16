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

    public static Transaction Create(CreateTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var description = ValidateEditableFields(
            request.OccurredOn, request.Amount, request.Description, request.AccountId, request.CategoryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EnteredByUserId);

        return new Transaction
        {
            Id = Guid.NewGuid(),
            OccurredOn = request.OccurredOn,
            Amount = request.Amount,
            Description = description,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            EnteredByUserId = request.EnteredByUserId,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Edit(EditTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var description = ValidateEditableFields(
            request.OccurredOn, request.Amount, request.Description, request.AccountId, request.CategoryId);

        OccurredOn = request.OccurredOn;
        Amount = request.Amount;
        Description = description;
        AccountId = request.AccountId;
        CategoryId = request.CategoryId;
        UpdatedUtc = DateTime.UtcNow;
    }

    private static string ValidateEditableFields(
        DateOnly occurredOn, decimal amount, string description, Guid accountId, Guid categoryId)
    {
        if (occurredOn > DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
            throw new ArgumentOutOfRangeException(nameof(occurredOn), "OccurredOn cannot be more than one day in the future.");

        if (amount == 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-zero.");

        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        var trimmed = description.Trim();
        if (trimmed.Length > 256)
            throw new ArgumentException("Description must be 256 characters or fewer.", nameof(description));

        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId is required.", nameof(accountId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is required.", nameof(categoryId));

        return trimmed;
    }
}
