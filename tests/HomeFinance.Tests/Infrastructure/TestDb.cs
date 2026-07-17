using HomeFinance.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HomeFinance.Tests.Infrastructure;

/// <summary>
/// Provides a per-test in-memory SQLite database with migrations applied.
/// The connection is held open for the lifetime of the instance so that
/// the in-memory database persists across multiple context instances.
/// </summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;

    public HomeFinanceDbContext Context { get; }

    public TestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Context = BuildContext();
        Context.Database.Migrate();
    }

    /// <summary>
    /// Returns a fresh <see cref="HomeFinanceDbContext"/> on the same connection.
    /// Needed for FK-restrict delete tests where the seeding context holds tracked
    /// navigations that would trip EF's cascade path before SQL runs.
    /// </summary>
    public HomeFinanceDbContext CreateContext() => BuildContext();

    private HomeFinanceDbContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<HomeFinanceDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new HomeFinanceDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
