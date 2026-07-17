using HomeFinance.Core.Contracts.Categories;
using HomeFinance.Core.Validation;

namespace HomeFinance.Core.Entities;

public sealed class Category
{
    private readonly List<Transaction> _transactions = [];

    // EF Core hydration only
    private Category() { }

    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public string ColorHex { get; private set; } = string.Empty;
    public bool IsArchived { get; private set; }
    public DateTime CreatedUtc { get; init; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public static Category Create(CategoryData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        data = CategoryDataValidator.Invoke(data);
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = data.Name,
            // CategoryDataValidator always sets ColorHex to a non-null value (default or validated).
            ColorHex = data.ColorHex!,
            Icon = data.Icon,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void Rename(string name)
    {
        Name = Rules.RequireLabel(name, 64, nameof(Name));
    }

    public void ChangeColor(string colorHex)
    {
        ColorHex = Rules.RequireHexColor(colorHex, nameof(ColorHex));
    }

    public void ChangeIcon(string? icon)
    {
        Icon = Rules.RequireOptionalLabel(icon, 128, nameof(Icon));
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
