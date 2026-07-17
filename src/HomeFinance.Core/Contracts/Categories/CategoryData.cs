namespace HomeFinance.Core.Contracts.Categories;

public sealed record CategoryData
{
    public required string Name { get; init; }
    public string? ColorHex { get; init; }
    public string? Icon { get; init; }
}
