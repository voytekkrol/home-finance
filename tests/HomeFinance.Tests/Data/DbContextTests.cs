using HomeFinance.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HomeFinance.Tests.Data;

/// <summary>
/// Integration tests that exercise the EF Core / SQLite layer.
/// Each test creates its own <see cref="TestDb"/> instance so there is no
/// shared mutable state between tests.
/// </summary>
public sealed class DbContextTests
{
    // -------------------------------------------------------------------------
    // Migration smoke test
    // -------------------------------------------------------------------------

    [Fact]
    public void Migrate_OnFreshDatabase_Succeeds()
    {
        // TestDb runs Migrate() in its constructor; reaching here means it worked.
        using var db = new TestDb();

        Assert.NotNull(db.Context.Accounts);
        Assert.NotNull(db.Context.Categories);
        Assert.NotNull(db.Context.Transactions);
    }

    [Fact]
    public void DbSets_AfterMigration_AreQueryable()
    {
        using var db = new TestDb();

        var accounts = db.Context.Accounts.ToList();
        var categories = db.Context.Categories.ToList();
        var transactions = db.Context.Transactions.ToList();

        Assert.Empty(accounts);
        Assert.Empty(categories);
        Assert.Empty(transactions);
    }

    // -------------------------------------------------------------------------
    // Category.Name unique index
    // -------------------------------------------------------------------------

    [Fact]
    public void SaveChanges_TwoCategoriesWithSameName_ThrowsDbUpdateException()
    {
        using var db = new TestDb();

        TestBed.CreateCategory(db.Context, "Groceries");

        var ex = Assert.Throws<DbUpdateException>(() =>
            TestBed.CreateCategory(db.Context, "Groceries"));

        Assert.NotNull(ex);
    }

    // -------------------------------------------------------------------------
    // Account.Name unique index
    // -------------------------------------------------------------------------

    [Fact]
    public void SaveChanges_TwoAccountsWithSameName_ThrowsDbUpdateException()
    {
        using var db = new TestDb();
        var user = TestBed.CreateUser(db.Context);

        TestBed.CreateAccount(db.Context, user.Id, "My Account");

        var ex = Assert.Throws<DbUpdateException>(() =>
            TestBed.CreateAccount(db.Context, user.Id, "My Account"));

        Assert.NotNull(ex);
    }

    // -------------------------------------------------------------------------
    // Account.OwnerUserId FK restrict
    // -------------------------------------------------------------------------

    [Fact]
    public void DeleteUser_WithOwnedAccount_ThrowsDbUpdateException()
    {
        using var db = new TestDb();
        var user = TestBed.CreateUser(db.Context);
        TestBed.CreateAccount(db.Context, user.Id);

        // Use a fresh context so EF does not perform an in-memory cascade
        using var ctx2 = db.CreateContext();
        var trackedUser = ctx2.Users.Single(u => u.Id == user.Id);
        ctx2.Users.Remove(trackedUser);

        Assert.Throws<DbUpdateException>(() => ctx2.SaveChanges());
    }

    // -------------------------------------------------------------------------
    // Transaction.AccountId FK restrict
    // -------------------------------------------------------------------------

    [Fact]
    public void DeleteAccount_ReferencedByTransaction_ThrowsDbUpdateException()
    {
        using var db = new TestDb();
        var user = TestBed.CreateUser(db.Context);
        var account = TestBed.CreateAccount(db.Context, user.Id);
        var category = TestBed.CreateCategory(db.Context);
        TestBed.CreateTransaction(db.Context, account.Id, category.Id, user.Id);

        using var ctx2 = db.CreateContext();
        var trackedAccount = ctx2.Accounts.Single(a => a.Id == account.Id);
        ctx2.Accounts.Remove(trackedAccount);

        Assert.Throws<DbUpdateException>(() => ctx2.SaveChanges());
    }

    // -------------------------------------------------------------------------
    // Transaction.CategoryId FK restrict
    // -------------------------------------------------------------------------

    [Fact]
    public void DeleteCategory_ReferencedByTransaction_ThrowsDbUpdateException()
    {
        using var db = new TestDb();
        var user = TestBed.CreateUser(db.Context);
        var account = TestBed.CreateAccount(db.Context, user.Id);
        var category = TestBed.CreateCategory(db.Context);
        TestBed.CreateTransaction(db.Context, account.Id, category.Id, user.Id);

        using var ctx2 = db.CreateContext();
        var trackedCategory = ctx2.Categories.Single(c => c.Id == category.Id);
        ctx2.Categories.Remove(trackedCategory);

        Assert.Throws<DbUpdateException>(() => ctx2.SaveChanges());
    }

    // -------------------------------------------------------------------------
    // Transaction.EnteredByUserId FK restrict
    // -------------------------------------------------------------------------

    [Fact]
    public void DeleteUser_WhoEnteredTransaction_ThrowsDbUpdateException()
    {
        using var db = new TestDb();
        var user = TestBed.CreateUser(db.Context);
        var account = TestBed.CreateAccount(db.Context, user.Id);
        var category = TestBed.CreateCategory(db.Context);
        TestBed.CreateTransaction(db.Context, account.Id, category.Id, user.Id);

        using var ctx2 = db.CreateContext();
        var trackedUser = ctx2.Users.Single(u => u.Id == user.Id);
        ctx2.Users.Remove(trackedUser);

        Assert.Throws<DbUpdateException>(() => ctx2.SaveChanges());
    }
}
