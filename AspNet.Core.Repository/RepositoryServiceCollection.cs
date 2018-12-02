using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Repository
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUnitOfWorkRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}
