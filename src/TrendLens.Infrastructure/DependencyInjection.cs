using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TrendLens.Infrastructure.Data;
using TrendLens.Infrastructure.Repositories;
using TrendLens.Infrastructure.Services;
using TrendLens.Core.Interfaces;

namespace TrendLens.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TrendLensDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(TrendLensDbContext).Assembly.FullName)));

        services.AddScoped<ISalesRepository, SalesRepository>();
        services.AddScoped<ICsvImportService, CsvImportService>();
        
        return services;
    }
}
