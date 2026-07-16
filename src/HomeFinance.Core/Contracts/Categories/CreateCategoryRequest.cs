namespace HomeFinance.Core.Contracts.Categories;

public sealed record CreateCategoryRequest
{
    public required string Name { get; init; }
    public string? ColorHex { get; init; }
    public string? Icon { get; init; }
}
