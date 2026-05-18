using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            connectionString = connectionString?
                .Replace("$DB_HOST", Environment.GetEnvironmentVariable("DB_HOST"))
                .Replace("$DB_PASSWORD", Environment.GetEnvironmentVariable("DB_PASSWORD"));

            services.AddTransient<IProductsRepository, ProductsRepository>();
            services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(connectionString));

            return services;
        }
    }
}
