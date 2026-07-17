using HomeFinance.Core.Contracts.Accounts;
using HomeFinance.Core.Contracts.Categories;
using HomeFinance.Core.Contracts.Transactions;
using HomeFinance.Core.Contracts.Users;
using HomeFinance.Core.Entities;
using HomeFinance.Core.Money;
using HomeFinance.Data;

namespace HomeFinance.Tests.Infrastructure;

/// <summary>
/// Factory helpers that construct entities via their domain factories and
/// persist them to the supplied context.  Every helper uses unique default
/// names so tests that call multiple helpers never collide on unique indexes.
/// </summary>
public static class TestBed
{
    /// <summary>
    /// Inserts an <see cref="ApplicationUser"/> directly into the database.
    /// Bypasses UserManager so that constraint tests can run without the full
    /// ASP.NET Core Identity service stack.
    /// </summary>
    public static ApplicationUser CreateUser(HomeFinanceDbContext ctx)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var user = ApplicationUser.Create(new CreateApplicationUserRequest
        {
            UserName = $"user_{suffix}",
            DisplayName = $"User {suffix}",
        });

        // Identity plumbing fields that UserManager would normally populate
        user.Id = Guid.NewGuid().ToString();
        user.NormalizedUserName = $"USER_{suffix.ToUpperInvariant()}";
        user.Email = $"user_{suffix}@test.local";
        user.NormalizedEmail = $"USER_{suffix.ToUpperInvariant()}@TEST.LOCAL";
        user.SecurityStamp = suffix;
        user.EmailConfirmed = true;

        ctx.Users.Add(user);
        ctx.SaveChanges();

        return user;
    }

    /// <summary>
    /// Creates and saves an <see cref="Account"/> owned by the specified user.
    /// A unique name is generated when <paramref name="name"/> is null.
    /// </summary>
    public static Account CreateAccount(
        HomeFinanceDbContext ctx,
        string ownerUserId,
        string? name = null)
    {
        name ??= $"Account_{Guid.NewGuid().ToString("N")[..8]}";

        var account = Account.Create(new CreateAccountRequest
        {
            Name = name,
            OwnerUserId = ownerUserId,
            Type = AccountType.Current,
            Currency = Currencies.Pln,
            OpeningBalance = 0m,
        });

        ctx.Accounts.Add(account);
        ctx.SaveChanges();

        return account;
    }

    /// <summary>
    /// Creates and saves a <see cref="Category"/>.
    /// A unique name is generated when <paramref name="name"/> is null.
    /// </summary>
    public static Category CreateCategory(HomeFinanceDbContext ctx, string? name = null)
    {
        name ??= $"Category_{Guid.NewGuid().ToString("N")[..8]}";

        var category = Category.Create(new CreateCategoryRequest { Name = name });

        ctx.Categories.Add(category);
        ctx.SaveChanges();

        return category;
    }

    /// <summary>
    /// Creates and saves a <see cref="Transaction"/> referencing the supplied
    /// account, category, and user.
    /// </summary>
    public static Transaction CreateTransaction(
        HomeFinanceDbContext ctx,
        Guid accountId,
        Guid categoryId,
        string enteredByUserId,
        decimal amount = -50m)
    {
        var transaction = Transaction.Create(new CreateTransactionRequest
        {
            OccurredOn = DateOnly.FromDateTime(DateTime.UtcNow),
            Amount = amount,
            Description = "Test transaction",
            AccountId = accountId,
            CategoryId = categoryId,
            EnteredByUserId = enteredByUserId,
        });

        ctx.Transactions.Add(transaction);
        ctx.SaveChanges();

        return transaction;
    }
}
