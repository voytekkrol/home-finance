namespace HomeFinance.Web.Authentication;

public interface IUserSeeder
{
    Task SeedAsync(CancellationToken ct);
}
