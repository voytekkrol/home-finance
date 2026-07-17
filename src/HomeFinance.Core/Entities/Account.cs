using HomeFinance.Core.Contracts.Accounts;
using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Entities;

public sealed class Account
{
    private readonly List<Transaction> _transactions = [];

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

    public static Account Create(AccountData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        data = AccountDataValidator.Invoke(data);
        return new Account
        {
            Id = Guid.NewGuid(),
            Name = data.Name,
            OwnerUserId = data.OwnerUserId,
            Type = data.Type,
            Currency = data.Currency,
            OpeningBalance = data.OpeningBalance,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Rename(string name)
    {
        Name = Rules.RequireLabel(name, 64, nameof(Name));
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
