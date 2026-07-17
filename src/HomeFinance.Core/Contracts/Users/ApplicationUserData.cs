namespace HomeFinance.Core.Contracts.Users;

public sealed record ApplicationUserData
{
    public required string UserName { get; init; }
    public required string DisplayName { get; init; }
}
