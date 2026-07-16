using HomeFinance.Core.Contracts.Accounts;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Money;

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
        var request = new CreateAccountRequest
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
    public void Create_NameIsNull_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = null!,
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        Assert.ThrowsAny<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_NameIsWhiteSpace_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "   ",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_NameExceeds64Chars_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = new string('A', 65),
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_NameWithSurroundingWhitespace_IsTrimmed()
    {
        var request = new CreateAccountRequest
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
    public void Create_OwnerUserIdIsNull_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = null!,
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        Assert.ThrowsAny<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_OwnerUserIdIsWhiteSpace_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "  ",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    // -------------------------------------------------------------------------
    // Create — AccountType validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_UndefinedAccountType_ThrowsArgumentOutOfRangeException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = (AccountType)999,
            Currency = Currencies.Pln,
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => Account.Create(request));
    }

    // -------------------------------------------------------------------------
    // Create — Currency validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_CurrencyIsNull_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = null!,
        };

        Assert.ThrowsAny<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_CurrencyIsWhiteSpace_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "   ",
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_CurrencyFewerThan3Chars_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PL",
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_CurrencyMoreThan3Chars_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PLNN",
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_CurrencyContainsNonLetter_ThrowsArgumentException()
    {
        var request = new CreateAccountRequest
        {
            Name = "Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = "PL1",
        };

        Assert.Throws<ArgumentException>(() => Account.Create(request));
    }

    [Fact]
    public void Create_LowercaseCurrency_NormalizesToUpperCase()
    {
        var request = new CreateAccountRequest
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
        var request = new CreateAccountRequest
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
    public void Rename_InvalidName_ThrowsAndLeavesNameUnchanged()
    {
        var account = Account.Create(ValidRequest());
        var originalName = account.Name;

        Assert.Throws<ArgumentException>(() => account.Rename("   "));

        Assert.Equal(originalName, account.Name);
    }

    [Fact]
    public void Rename_NameExceeds64Chars_ThrowsAndLeavesNameUnchanged()
    {
        var account = Account.Create(ValidRequest());
        var originalName = account.Name;

        Assert.Throws<ArgumentException>(() => account.Rename(new string('B', 65)));

        Assert.Equal(originalName, account.Name);
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

    private static CreateAccountRequest ValidRequest() =>
        new()
        {
            Name = "Test Account",
            OwnerUserId = "user-1",
            Type = AccountType.Current,
            Currency = Currencies.Pln,
            OpeningBalance = 0m,
        };
}
