using System.Text.RegularExpressions;
using HomeFinance.Core.Categorization;

namespace HomeFinance.Core.Entities;

public sealed partial class Category
{
    private List<Transaction> _transactions = [];

    // EF Core hydration only
    private Category() { }

    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string? Icon { get; private set; }
    public string ColorHex { get; private set; } = string.Empty;
    public bool IsArchived { get; private set; }
    public DateTime CreatedUtc { get; init; }
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$")]
    private static partial Regex ColorHexRegex();

    public static Category Create(string name, string? colorHex = null, string? icon = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name.Trim();
        if (name.Length > 64)
            throw new ArgumentException("Name must be 64 characters or fewer.", nameof(name));

        string resolvedColor;
        if (string.IsNullOrWhiteSpace(colorHex))
        {
            resolvedColor = Colors.DefaultCategory;
        }
        else
        {
            if (!ColorHexRegex().IsMatch(colorHex))
                throw new ArgumentException("ColorHex must be in #RRGGBB format.", nameof(colorHex));
            resolvedColor = "#" + colorHex[1..].ToUpperInvariant();
        }

        string? resolvedIcon = null;
        if (!string.IsNullOrWhiteSpace(icon))
        {
            icon = icon.Trim();
            if (icon.Length > 128)
                throw new ArgumentException("Icon must be 128 characters or fewer.", nameof(icon));
            resolvedIcon = icon;
        }

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            ColorHex = resolvedColor,
            Icon = resolvedIcon,
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

    public void ChangeColor(string colorHex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(colorHex);
        if (!ColorHexRegex().IsMatch(colorHex))
            throw new ArgumentException("ColorHex must be in #RRGGBB format.", nameof(colorHex));

        ColorHex = "#" + colorHex[1..].ToUpperInvariant();
    }

    public void ChangeIcon(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon))
        {
            Icon = null;
            return;
        }

        icon = icon.Trim();
        if (icon.Length > 128)
            throw new ArgumentException("Icon must be 128 characters or fewer.", nameof(icon));

        Icon = icon;
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
