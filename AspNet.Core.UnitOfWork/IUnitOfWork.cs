using System;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Core.Repository;

namespace AspNetCore.UnitOfWork
{
    public interface IUnitOfWork : IUnitOfWorkForService, IQueryExecutor
    {
        int Save(bool acceptAllChangesOnSuccess);
        int Save();
        Task<int> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        void Dispose(bool disposing);
        void Dispose();
    }

    public interface IUnitOfWorkForService
    {
        IGenericRepository<T> Repository<T>() where T : class, new();

    }
}
