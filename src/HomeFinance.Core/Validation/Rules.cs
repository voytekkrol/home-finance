using System.Text.RegularExpressions;

namespace HomeFinance.Core.Validation;

public static partial class Rules
{
    [GeneratedRegex(@"^#[0-9A-F]{6}$")]
    private static partial Regex HexColorRegex();

    public static string RequireLabel(string? value, int maxLength, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new MissingRequiredValueException(paramName);

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
            throw new LabelTooLongException(paramName);

        return trimmed;
    }

    public static string? RequireOptionalLabel(string? value, int maxLength, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
            throw new LabelTooLongException(paramName);

        return trimmed;
    }

    public static string RequireIsoCurrencyCode(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new MissingRequiredValueException(paramName);

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length != 3 || !normalized.All(c => c is >= 'A' and <= 'Z'))
            throw new InvalidCurrencyCodeException(paramName);

        return normalized;
    }

    public static string RequireHexColor(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new MissingRequiredValueException(paramName);

        var trimmed = value.Trim();

        // Normalize to upper-case before regex check (regex only accepts A-F upper-case).
        var normalized = trimmed.Length >= 2
            ? trimmed[..1] + trimmed[1..].ToUpperInvariant()
            : trimmed;

        if (!HexColorRegex().IsMatch(normalized))
            throw new InvalidHexColorException(paramName);

        return normalized;
    }

    public static TEnum RequireDefined<TEnum>(TEnum value, string paramName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new InvalidEnumValueException(paramName);

        return value;
    }

    public static string RequireIdentityUserId(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidIdentityUserIdException(paramName);

        return value.Trim();
    }

    public static Guid RequireNonEmptyGuid(Guid value, string paramName)
    {
        if (value == Guid.Empty)
            throw new EmptyGuidException(paramName);

        return value;
    }

    public static decimal RequireNonZero(decimal value, string paramName)
    {
        if (value == 0m)
            throw new ZeroAmountException(paramName);

        return value;
    }

    public static DateOnly RequireNotFarFuture(DateOnly value, int maxDaysAhead, string paramName)
    {
        if (value > DateOnly.FromDateTime(DateTime.UtcNow).AddDays(maxDaysAhead))
            throw new FutureDateException(paramName);

        return value;
    }
}
