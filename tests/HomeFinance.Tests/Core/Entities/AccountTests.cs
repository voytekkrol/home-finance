using HomeFinance.Core.Contracts.Accounts;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Money;
using HomeFinance.Core.Validation;

namespace HomeFinance.Tests.Core.Entities;

public sealed class AccountTests
{
    // -------------------------------------------------------------------------
    // Create — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_ValidRequest_ReturnsAccountWithGeneratedId()
    {
        var request = ValidRequest();

        var account = Account.Create(request);

        Assert.NotEqual(Guid.Empty, account.Id);
    }

    [Fact]
    public void Create_ValidRequest_SetsCreatedUtcToApproximatelyNow()
    {
        var before = DateTime.UtcNow;

        var account = Account.Create(ValidRequest());

        var after = DateTime.UtcNow;
        Assert.InRange(account.CreatedUtc, before, after);
    }

    [Fact]
    public void Create_ValidRequest_PopulatesAllFieldsFromRequest()
    {
        var request = new AccountData
        {
            Name = "My Account",
            OwnerUserId = "user-1",
            Type = AccountType.Savings,
            Currency = Currencies.Pln,
            OpeningBalance = 100.50m,
        };

        var account = Account.Create(request);

        Assert.Equal("My Account", account.Name);
        Assert.Equal("user-1", account.OwnerUserId);
        Assert.Equal(AccountType.Savings, account.Type);
        Assert.Equal(Currencies.Pln, account.Currency);
        Assert.Equal(100.50m, account.OpeningBalance);
    }

    [Fact]
    public void Create_ValidRequest_IsArchivedIsFalse()
    {
        var account = Account.Create(ValidRequest());

        Assert.False(account.IsArchived);
    }

    [Fact]
    public void Create_ValidRequest_TransactionsCollectionIsEmpty()
    {
        var account = Account.Create(ValidRequest());

        Assert.Empty(account.Transactions);
    }

    // -------------------------------------------------------------------------
    // Create — null request
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Account.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — Name validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NameIsNull_ThrowsMissingRequiredValueException()
    {
        var request = new AccountData
        {
            Name = null!,
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Name), ex.ParamName);
    }

    [Fact]
    public void Create_NameIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var request = new AccountData
        {
            Name = "   ",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Name), ex.ParamName);
    }

    [Fact]
    public void Create_NameExceeds64Chars_ThrowsLabelTooLongException()
    {
        var request = new AccountData
        {
            Name = new string('A', 65),
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<LabelTooLongException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Name), ex.ParamName);
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_IsTrimmed()
    {
        var request = new AccountData
        {
            Name = "  My Account  ",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var account = Account.Create(request);

        Assert.Equal("My Account", account.Name);
    }

    // -------------------------------------------------------------------------
    // Create — OwnerUserId validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_OwnerUserIdIsNull_ThrowsInvalidIdentityUserIdException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = null!,
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<InvalidIdentityUserIdException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.OwnerUserId), ex.ParamName);
    }

    [Fact]
    public void Create_OwnerUserIdIsWhiteSpace_ThrowsInvalidIdentityUserIdException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "  ",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<InvalidIdentityUserIdException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.OwnerUserId), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — AccountType validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_UndefinedAccountType_ThrowsInvalidEnumValueException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = (AccountType)999,
            Currency = Currencies.Pln,
        };

        var ex = Assert.Throws<InvalidEnumValueException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Type), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — Currency validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_CurrencyIsNull_ThrowsMissingRequiredValueException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = null!,
        };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Currency), ex.ParamName);
    }

    [Fact]
    public void Create_CurrencyIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "   ",
        };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Currency), ex.ParamName);
    }

    [Fact]
    public void Create_CurrencyFewerThan3Chars_ThrowsInvalidCurrencyCodeException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PL",
        };

        var ex = Assert.Throws<InvalidCurrencyCodeException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Currency), ex.ParamName);
    }

    [Fact]
    public void Create_CurrencyMoreThan3Chars_ThrowsInvalidCurrencyCodeException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PLNN",
        };

        var ex = Assert.Throws<InvalidCurrencyCodeException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Currency), ex.ParamName);
    }

    [Fact]
    public void Create_CurrencyContainsNonLetter_ThrowsInvalidCurrencyCodeException()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PL1",
        };

        var ex = Assert.Throws<InvalidCurrencyCodeException>(() => Account.Create(request));

        Assert.Equal(nameof(AccountData.Currency), ex.ParamName);
    }

    [Fact]
    public void Create_LowercaseCurrency_NormalizesToUpperCase()
    {
        var request = new AccountData
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "pln",
        };

        var account = Account.Create(request);

        Assert.Equal("PLN", account.Currency);
    }

    // -------------------------------------------------------------------------
    // Create — OpeningBalance
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NegativeOpeningBalance_IsAccepted()
    {
        var request = new AccountData
        {
            Name = "Credit Card",
            OwnerUserId = "user-1",
            Type = AccountType.Credit,
            Currency = Currencies.Pln,
            OpeningBalance = -500m,
        };

        var account = Account.Create(request);

        Assert.Equal(-500m, account.OpeningBalance);
    }

    // -------------------------------------------------------------------------
    // Rename
    // -------------------------------------------------------------------------

    [Fact]
    public void Rename_ValidName_ChangesName()
    {
        var account = Account.Create(ValidRequest());

        account.Rename("New Name");

        Assert.Equal("New Name", account.Name);
    }

    [Fact]
    public void Rename_WhiteSpaceName_ThrowsMissingRequiredValueExceptionAndLeavesNameUnchanged()
    {
        var account = Account.Create(ValidRequest());
        var originalName = account.Name;

        var ex = Assert.Throws<MissingRequiredValueException>(() => account.Rename("   "));

        Assert.Equal(originalName, account.Name);
        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public void Rename_NameExceeds64Chars_ThrowsLabelTooLongExceptionAndLeavesNameUnchanged()
    {
        var account = Account.Create(ValidRequest());
        var originalName = account.Name;

        var ex = Assert.Throws<LabelTooLongException>(() => account.Rename(new string('B', 65)));

        Assert.Equal(originalName, account.Name);
        Assert.Equal("name", ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Archive / Unarchive
    // -------------------------------------------------------------------------

    [Fact]
    public void Archive_WhenNotArchived_SetsIsArchivedTrue()
    {
        var account = Account.Create(ValidRequest());

        account.Archive();

        Assert.True(account.IsArchived);
    }

    [Fact]
    public void Archive_WhenAlreadyArchived_IsNoOp()
    {
        var account = Account.Create(ValidRequest());
        account.Archive();

        account.Archive();

        Assert.True(account.IsArchived);
    }

    [Fact]
    public void Unarchive_WhenArchived_SetsIsArchivedFalse()
    {
        var account = Account.Create(ValidRequest());
        account.Archive();

        account.Unarchive();

        Assert.False(account.IsArchived);
    }

    [Fact]
    public void Unarchive_WhenNotArchived_IsNoOp()
    {
        var account = Account.Create(ValidRequest());

        account.Unarchive();

        Assert.False(account.IsArchived);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static AccountData ValidRequest() =>
        new()
        {
            Name = "Test Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
            OpeningBalance = 0m,
        };
}
