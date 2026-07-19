namespace HomeFinance.Web.Categorization;

public interface ICategorySeeder
{
    Task SeedAsync(CancellationToken ct);
}
