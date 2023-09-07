using Microsoft.EntityFrameworkCore;

namespace Producer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TestDatabase") ??
            throw new ArgumentNullException("Failed reading connection string. Ensure configuration is correct.");

            services.AddDbContext<ProducerContext>(options => options.UseSqlServer(connectionString, opt => opt.EnableRetryOnFailure()));

            return services;
        }
    }
}
