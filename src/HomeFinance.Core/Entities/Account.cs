namespace HomeFinance.Core.Entities;

public sealed class Account
{
    private List<Transaction> _transactions = [];

    // EF Core hydration only
    private Account() { }

    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string OwnerUserId { get; init; } = string.Empty;
    public AccountType Type { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal OpeningBalance { get; init; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedUtc { get; init; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public static Account Create(
        string name,
        string ownerUserId,
        AccountType type,
        string currency,
        decimal openingBalance)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name.Trim();
        if (name.Length > 64)
            throw new ArgumentException("Name must be 64 characters or fewer.", nameof(name));

        ArgumentException.ThrowIfNullOrWhiteSpace(ownerUserId);

        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));

        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        currency = currency.Trim().ToUpperInvariant();
        if (currency.Length != 3 || !currency.All(c => c is >= 'A' and <= 'Z'))
            throw new ArgumentException("Currency must be an ISO-4217 three-letter code.", nameof(currency));

        return new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            OwnerUserId = ownerUserId,
            Type = type,
            Currency = currency,
            OpeningBalance = openingBalance,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name.Trim();
        if (name.Length > 64)
            throw new ArgumentException("Name must be 64 characters or fewer.", nameof(name));

        Name = name;
    }

    public void Archive()
    {
        if (!IsArchived)
            IsArchived = true;
    }

    public void Unarchive()
    {
        if (IsArchived)
            IsArchived = false;
    }
}
