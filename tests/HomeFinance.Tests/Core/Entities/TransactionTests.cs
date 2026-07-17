using HomeFinance.Core.Contracts.Transactions;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Validation;

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
    public void Create_ValidData_ReturnsTransactionWithGeneratedId()
    {
        var transaction = Transaction.Create(ValidCreateData());

        Assert.NotEqual(Guid.Empty, transaction.Id);
    }

    [Fact]
    public void Create_ValidData_SetsCreatedUtcToApproximatelyNow()
    {
        var before = DateTime.UtcNow;

        var transaction = Transaction.Create(ValidCreateData());

        var after = DateTime.UtcNow;
        Assert.InRange(transaction.CreatedUtc, before, after);
    }

    [Fact]
    public void Create_ValidData_UpdatedUtcIsNull()
    {
        var transaction = Transaction.Create(ValidCreateData());

        Assert.Null(transaction.UpdatedUtc);
    }

    [Fact]
    public void Create_ValidData_PopulatesAllFieldsFromData()
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
    // Create — null data
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Transaction.Create(null!));
    }

    // -------------------------------------------------------------------------
    // Create — OccurredOn validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_OccurredOnMoreThan1DayInFuture_ThrowsFutureDateException()
    {
        var request = ValidCreateData() with
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2),
        };

        var ex = Assert.Throws<FutureDateException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.OccurredOn), ex.ParamName);
    }

    [Fact]
    public void Create_OccurredOnIsToday_Succeeds()
    {
        var request = ValidCreateData() with
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
        };

        var transaction = Transaction.Create(request);

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), transaction.OccurredOn);
    }

    [Fact]
    public void Create_OccurredOnIsTomorrow_Succeeds()
    {
        var request = ValidCreateData() with
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
    public void Create_AmountIsZero_ThrowsZeroAmountException()
    {
        var request = ValidCreateData() with { Amount = 0m };

        var ex = Assert.Throws<ZeroAmountException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.Amount), ex.ParamName);
    }

    [Fact]
    public void Create_PositiveAmount_Succeeds()
    {
        var request = ValidCreateData() with { Amount = 100m };

        var transaction = Transaction.Create(request);

        Assert.Equal(100m, transaction.Amount);
    }

    [Fact]
    public void Create_NegativeAmount_Succeeds()
    {
        var request = ValidCreateData() with { Amount = -100m };

        var transaction = Transaction.Create(request);

        Assert.Equal(-100m, transaction.Amount);
    }

    // -------------------------------------------------------------------------
    // Create — Description validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_DescriptionIsNull_ThrowsMissingRequiredValueException()
    {
        var request = ValidCreateData() with { Description = null! };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.Description), ex.ParamName);
    }

    [Fact]
    public void Create_DescriptionIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var request = ValidCreateData() with { Description = "   " };

        var ex = Assert.Throws<MissingRequiredValueException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.Description), ex.ParamName);
    }

    [Fact]
    public void Create_DescriptionExceeds256Chars_ThrowsLabelTooLongException()
    {
        var request = ValidCreateData() with { Description = new string('D', 257) };

        var ex = Assert.Throws<LabelTooLongException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.Description), ex.ParamName);
    }

    [Fact]
    public void Create_DescriptionWithSurroundingWhitespace_IsTrimmed()
    {
        var request = ValidCreateData() with { Description = "  Coffee  " };

        var transaction = Transaction.Create(request);

        Assert.Equal("Coffee", transaction.Description);
    }

    // -------------------------------------------------------------------------
    // Create — AccountId / CategoryId validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_AccountIdIsEmpty_ThrowsEmptyGuidException()
    {
        var request = ValidCreateData() with { AccountId = Guid.Empty };

        var ex = Assert.Throws<EmptyGuidException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.AccountId), ex.ParamName);
    }

    [Fact]
    public void Create_CategoryIdIsEmpty_ThrowsEmptyGuidException()
    {
        var request = ValidCreateData() with { CategoryId = Guid.Empty };

        var ex = Assert.Throws<EmptyGuidException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.CategoryId), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Create — EnteredByUserId validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Create_EnteredByUserIdIsNull_ThrowsInvalidIdentityUserIdException()
    {
        var request = ValidCreateData() with { EnteredByUserId = null! };

        var ex = Assert.Throws<InvalidIdentityUserIdException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.EnteredByUserId), ex.ParamName);
    }

    [Fact]
    public void Create_EnteredByUserIdIsWhiteSpace_ThrowsInvalidIdentityUserIdException()
    {
        var request = ValidCreateData() with { EnteredByUserId = "   " };

        var ex = Assert.Throws<InvalidIdentityUserIdException>(() => Transaction.Create(request));

        Assert.Equal(nameof(CreateTransactionData.EnteredByUserId), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Edit — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_ValidData_UpdatesAllEditableFields()
    {
        var transaction = Transaction.Create(ValidCreateData());
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
    public void Edit_ValidData_SetsUpdatedUtcToApproximatelyNow()
    {
        var transaction = Transaction.Create(ValidCreateData());
        var before = DateTime.UtcNow;

        transaction.Edit(ValidEditData());

        var after = DateTime.UtcNow;
        Assert.NotNull(transaction.UpdatedUtc);
        Assert.InRange(transaction.UpdatedUtc!.Value, before, after);
    }

    // -------------------------------------------------------------------------
    // Edit — re-runs all Create validations (spot-checks, tightened)
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_AmountIsZero_ThrowsZeroAmountException()
    {
        var transaction = Transaction.Create(ValidCreateData());

        var ex = Assert.Throws<ZeroAmountException>(() =>
            transaction.Edit(ValidEditData() with { Amount = 0m }));

        Assert.Equal(nameof(EditTransactionData.Amount), ex.ParamName);
    }

    [Fact]
    public void Edit_OccurredOnMoreThan1DayInFuture_ThrowsFutureDateException()
    {
        var transaction = Transaction.Create(ValidCreateData());

        var ex = Assert.Throws<FutureDateException>(() =>
            transaction.Edit(ValidEditData() with
            {
                OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2),
            }));

        Assert.Equal(nameof(EditTransactionData.OccurredOn), ex.ParamName);
    }

    [Fact]
    public void Edit_DescriptionIsWhiteSpace_ThrowsMissingRequiredValueException()
    {
        var transaction = Transaction.Create(ValidCreateData());

        var ex = Assert.Throws<MissingRequiredValueException>(() =>
            transaction.Edit(ValidEditData() with { Description = "   " }));

        Assert.Equal(nameof(EditTransactionData.Description), ex.ParamName);
    }

    [Fact]
    public void Edit_AccountIdIsEmpty_ThrowsEmptyGuidException()
    {
        var transaction = Transaction.Create(ValidCreateData());

        var ex = Assert.Throws<EmptyGuidException>(() =>
            transaction.Edit(ValidEditData() with { AccountId = Guid.Empty }));

        Assert.Equal(nameof(EditTransactionData.AccountId), ex.ParamName);
    }

    [Fact]
    public void Edit_CategoryIdIsEmpty_ThrowsEmptyGuidException()
    {
        var transaction = Transaction.Create(ValidCreateData());

        var ex = Assert.Throws<EmptyGuidException>(() =>
            transaction.Edit(ValidEditData() with { CategoryId = Guid.Empty }));

        Assert.Equal(nameof(EditTransactionData.CategoryId), ex.ParamName);
    }

    // -------------------------------------------------------------------------
    // Edit — failure leaves state unchanged
    // -------------------------------------------------------------------------

    [Fact]
    public void Edit_InvalidData_LeavesEntityStateUnchanged()
    {
        var transaction = Transaction.Create(ValidCreateData());
        var originalOccurredOn = transaction.OccurredOn;
        var originalAmount = transaction.Amount;
        var originalDescription = transaction.Description;
        var originalAccountId = transaction.AccountId;
        var originalCategoryId = transaction.CategoryId;
        var originalUpdatedUtc = transaction.UpdatedUtc;

        // Zero amount is invalid and will throw before any state is mutated
        Assert.Throws<ZeroAmountException>(() =>
            transaction.Edit(ValidEditData() with { Amount = 0m }));

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

    private static CreateTransactionData ValidCreateData() =>
        new()
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = -50m,
            Description = "Coffee",
            AccountId = SomeAccountId,
            CategoryId = SomeCategoryId,
            EnteredByUserId = SomeUserId,
        };

    private static EditTransactionData ValidEditData() =>
        new()
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = -50m,
            Description = "Coffee",
            AccountId = SomeAccountId,
            CategoryId = SomeCategoryId,
        };
}
