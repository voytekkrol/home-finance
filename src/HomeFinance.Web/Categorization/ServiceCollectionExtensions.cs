namespace HomeFinance.Web.Categorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCategorySeeder(this IServiceCollection services)
    {
        services.AddScoped<ICategorySeeder, CategorySeeder>();
        return services;
    }
}
