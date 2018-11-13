using AspNet.Core.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddTransient(typeof(IGenericRepository<>), provider => 
            {
                var context = provider.GetService<TContext>();
                object[] paramsArray = new object[] { context };
                var instance = Activator.CreateInstance(typeof(GenericRepository<>), paramsArray);
                return instance;
            });
         
            return services;
        }
        public static IServiceCollection AddUnitOfWorkRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
