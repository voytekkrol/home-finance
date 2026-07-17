namespace HomeFinance.Core.Contracts.Transactions;

public sealed record EditTransactionData
{
    public required DateOnly OccurredOn { get; init; }
    public required decimal Amount { get; init; }
    public required string Description { get; init; }
    public required Guid AccountId { get; init; }
    public required Guid CategoryId { get; init; }
}
