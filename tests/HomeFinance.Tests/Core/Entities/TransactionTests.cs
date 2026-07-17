using HomeFinance.Core.Contracts.Transactions;
using HomeFinance.Core.Entities;

namespace HomeFinance.Tests.Core.Entities;

public sealed class TransactionTests
{
    private static readonly Guid SomeAccountId = Guid.NewGuid();
    private static readonly Guid SomeCategoryId = Guid.NewGuid();
    private const string SomeUserId = "user-1";

    // -------------------------------------------------------------------------
    // Create — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_ValidRequest_ReturnsTransactionWithGeneratedId()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.NotEqual(Guid.Empty, transaction.Id);
    }

    [Fact]
    public void Create_ValidRequest_SetsCreatedUtcToApproximatelyNow()
    {
        var before = DateTime.UtcNow;

        var transaction = Transaction.Create(ValidCreateRequest());

        var after = DateTime.UtcNow;
        Assert.InRange(transaction.CreatedUtc, before, after);
    }

    [Fact]
    public void Create_ValidRequest_UpdatedUtcIsNull()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.Null(transaction.UpdatedUtc);
    }

    [Fact]
    public void Create_ValidRequest_PopulatesAllFieldsFromRequest()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new CreateTransactionData
        {
            OccurredOn = today,
            Amount = -75.00m,
            Description = "Lunch at office",
            AccountId = SomeAccountId,
            CategoryId = SomeCategoryId,
            EnteredByUserId = SomeUserId,
        };

        var transaction = Transaction.Create(request);

        Assert.Equal(today, transaction.OccurredOn);
        Assert.Equal(-75.00m, transaction.Amount);
        Assert.Equal("Lunch at office", transaction.Description);
        Assert.Equal(SomeAccountId, transaction.AccountId);
        Assert.Equal(SomeCategoryId, transaction.CategoryId);
        Assert.Equal(SomeUserId, transaction.EnteredByUserId);
    }

    // -------------------------------------------------------------------------
    // Create — null request
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Transaction.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — OccurredOn validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_OccurredOnMoreThan1DayInFuture_ThrowsArgumentOutOfRangeException()
    {
        var request = ValidCreateRequest() with
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2),
        };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_OccurredOnIsToday_Succeeds()
    {
        var request = ValidCreateRequest() with
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var transaction = Transaction.Create(request);

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), transaction.OccurredOn);
    }

    [Fact]
    public void Create_OccurredOnIsTomorrow_Succeeds()
    {
        var request = ValidCreateRequest() with
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
        };

        var transaction = Transaction.Create(request);

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1), transaction.OccurredOn);
    }

    // -------------------------------------------------------------------------
    // Create — Amount validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_AmountIsZero_ThrowsArgumentOutOfRangeException()
    {
        var request = ValidCreateRequest() with { Amount = 0m };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_PositiveAmount_Succeeds()
    {
        var request = ValidCreateRequest() with { Amount = 100m };

        var transaction = Transaction.Create(request);

        Assert.Equal(100m, transaction.Amount);
    }

    [Fact]
    public void Create_NegativeAmount_Succeeds()
    {
        var request = ValidCreateRequest() with { Amount = -100m };

        var transaction = Transaction.Create(request);

        Assert.Equal(-100m, transaction.Amount);
    }

    // -------------------------------------------------------------------------
    // Create — Description validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_DescriptionIsNull_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { Description = null! };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_DescriptionIsWhiteSpace_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { Description = "   " };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_DescriptionExceeds256Chars_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { Description = new string('D', 257) };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_DescriptionWithSurroundingWhitespace_IsTrimmed()
    {
        var request = ValidCreateRequest() with { Description = "  Coffee  " };

        var transaction = Transaction.Create(request);

        Assert.Equal("Coffee", transaction.Description);
    }

    // -------------------------------------------------------------------------
    // Create — AccountId / CategoryId validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_AccountIdIsEmpty_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { AccountId = Guid.Empty };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_CategoryIdIsEmpty_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { CategoryId = Guid.Empty };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    // -------------------------------------------------------------------------
    // Create — EnteredByUserId validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_EnteredByUserIdIsNull_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { EnteredByUserId = null! };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    [Fact]
    public void Create_EnteredByUserIdIsWhiteSpace_ThrowsArgumentException()
    {
        var request = ValidCreateRequest() with { EnteredByUserId = "   " };

        Assert.ThrowsAny<ArgumentException>(() => Transaction.Create(request));
    }

    // -------------------------------------------------------------------------
    // Edit — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_ValidRequest_UpdatesAllEditableFields()
    {
        var transaction = Transaction.Create(ValidCreateRequest());
        var newAccountId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var newDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        transaction.Edit(new EditTransactionData
        {
            OccurredOn = newDate,
            Amount = 200m,
            Description = "Updated description",
            AccountId = newAccountId,
            CategoryId = newCategoryId,
        });

        Assert.Equal(newDate, transaction.OccurredOn);
        Assert.Equal(200m, transaction.Amount);
        Assert.Equal("Updated description", transaction.Description);
        Assert.Equal(newAccountId, transaction.AccountId);
        Assert.Equal(newCategoryId, transaction.CategoryId);
    }

    [Fact]
    public void Edit_ValidRequest_SetsUpdatedUtcToApproximatelyNow()
    {
        var transaction = Transaction.Create(ValidCreateRequest());
        var before = DateTime.UtcNow;

        transaction.Edit(ValidEditRequest());

        var after = DateTime.UtcNow;
        Assert.NotNull(transaction.UpdatedUtc);
        Assert.InRange(transaction.UpdatedUtc!.Value, before, after);
    }

    // -------------------------------------------------------------------------
    // Edit — re-runs all Create validations (spot-checks)
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_AmountIsZero_ThrowsArgumentOutOfRangeException()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.ThrowsAny<ArgumentException>(() =>
            transaction.Edit(ValidEditRequest() with { Amount = 0m }));
    }

    [Fact]
    public void Edit_OccurredOnMoreThan1DayInFuture_ThrowsArgumentOutOfRangeException()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.ThrowsAny<ArgumentException>(() =>
            transaction.Edit(ValidEditRequest() with
            {
                OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2),
            }));
    }

    [Fact]
    public void Edit_DescriptionIsWhiteSpace_ThrowsArgumentException()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.ThrowsAny<ArgumentException>(() =>
            transaction.Edit(ValidEditRequest() with { Description = "   " }));
    }

    [Fact]
    public void Edit_AccountIdIsEmpty_ThrowsArgumentException()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.ThrowsAny<ArgumentException>(() =>
            transaction.Edit(ValidEditRequest() with { AccountId = Guid.Empty }));
    }

    [Fact]
    public void Edit_CategoryIdIsEmpty_ThrowsArgumentException()
    {
        var transaction = Transaction.Create(ValidCreateRequest());

        Assert.ThrowsAny<ArgumentException>(() =>
            transaction.Edit(ValidEditRequest() with { CategoryId = Guid.Empty }));
    }

    // -------------------------------------------------------------------------
    // Edit — failure leaves state unchanged
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_InvalidRequest_LeavesEntityStateUnchanged()
    {
        var transaction = Transaction.Create(ValidCreateRequest());
        var originalOccurredOn = transaction.OccurredOn;
        var originalAmount = transaction.Amount;
        var originalDescription = transaction.Description;
        var originalAccountId = transaction.AccountId;
        var originalCategoryId = transaction.CategoryId;
        var originalUpdatedUtc = transaction.UpdatedUtc;

        // Zero amount is invalid and will throw before any state is mutated
        try
        {
            transaction.Edit(ValidEditRequest() with { Amount = 0m });
        }
        catch (ArgumentException)
        {
            // expected
        }

        Assert.Equal(originalOccurredOn, transaction.OccurredOn);
        Assert.Equal(originalAmount, transaction.Amount);
        Assert.Equal(originalDescription, transaction.Description);
        Assert.Equal(originalAccountId, transaction.AccountId);
        Assert.Equal(originalCategoryId, transaction.CategoryId);
        Assert.Equal(originalUpdatedUtc, transaction.UpdatedUtc);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static CreateTransactionData ValidCreateRequest() =>
        new()
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = -50m,
            Description = "Coffee",
            AccountId = SomeAccountId,
            CategoryId = SomeCategoryId,
            EnteredByUserId = SomeUserId,
        };

    private static EditTransactionData ValidEditRequest() =>
        new()
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = -50m,
            Description = "Coffee",
            AccountId = SomeAccountId,
            CategoryId = SomeCategoryId,
        };
}
