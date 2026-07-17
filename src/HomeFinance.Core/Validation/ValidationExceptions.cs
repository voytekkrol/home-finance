namespace HomeFinance.Core.Validation;

public abstract class DomainValidationException : ArgumentException
{
    protected DomainValidationException(string message, string paramName)
        : base(message, paramName) { }
}

public sealed class MissingRequiredValueException : DomainValidationException
{
    public new const string Message = "Value is required.";

    public MissingRequiredValueException(string paramName)
        : base(Message, paramName) { }
}

public sealed class LabelTooLongException : DomainValidationException
{
    public new const string Message = "Label exceeds the allowed length.";

    public LabelTooLongException(string paramName)
        : base(Message, paramName) { }
}

public sealed class InvalidCurrencyCodeException : DomainValidationException
{
    public new const string Message = "Currency must be an ISO-4217 three-letter code.";

    public InvalidCurrencyCodeException(string paramName)
        : base(Message, paramName) { }
}

public sealed class InvalidHexColorException : DomainValidationException
{
    public new const string Message = "Color must be in #RRGGBB format.";

    public InvalidHexColorException(string paramName)
        : base(Message, paramName) { }
}

public sealed class InvalidEnumValueException : DomainValidationException
{
    public new const string Message = "Enum value is not defined.";

    public InvalidEnumValueException(string paramName)
        : base(Message, paramName) { }
}

public sealed class InvalidIdentityUserIdException : DomainValidationException
{
    public new const string Message = "Identity user id is required.";

    public InvalidIdentityUserIdException(string paramName)
        : base(Message, paramName) { }
}

public sealed class EmptyGuidException : DomainValidationException
{
    public new const string Message = "Guid must not be empty.";

    public EmptyGuidException(string paramName)
        : base(Message, paramName) { }
}

public sealed class ZeroAmountException : DomainValidationException
{
    public new const string Message = "Amount must be non-zero.";

    public ZeroAmountException(string paramName)
        : base(Message, paramName) { }
}

public sealed class FutureDateException : DomainValidationException
{
    public new const string Message = "Date is too far in the future.";

    public FutureDateException(string paramName)
        : base(Message, paramName) { }
}
