using AspNet.Core;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddUnitOfWorkRepository();

            services.AddTransient<IUnitOfWork>(provider => {
               var context = provider.GetService<TContext>();
               return new UnitOfWork(context);
            });

            return services;
        }
    }
}