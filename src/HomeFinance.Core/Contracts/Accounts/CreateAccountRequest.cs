using HomeFinance.Core.Entities;

namespace HomeFinance.Core.Contracts.Accounts;

public sealed record CreateAccountRequest
{
    public required string Name { get; init; }
    public required string OwnerUserId { get; init; }
    public required AccountType Type { get; init; }
    public required string Currency { get; init; }
    public decimal OpeningBalance { get; init; }
}
