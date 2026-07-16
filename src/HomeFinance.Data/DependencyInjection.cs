using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeFinance.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<HomeFinanceDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")
                ?? "Data Source=homefinance.db"));
        return services;
    }
}
