using AspNet.Core;
using AspNet.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddTransient(typeof(IGenericRepository<>),typeof(GenericRepository<>));

            services.AddTransient<IUnitOfWork>(provider => {
               var context = provider.GetService<TContext>();
               return new UnitOfWork(context);
            });

            return services;
        }
    }
}