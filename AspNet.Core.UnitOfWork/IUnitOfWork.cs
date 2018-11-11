using System;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Core.Repository;

namespace AspNet.Core
{
    public interface IUnitOfWork : IUnitOfWorkForService
    {
        int Save(bool acceptAllChangesOnSuccess);
        int Save();
        Task<int> Save(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> Save(CancellationToken cancellationToken = default(CancellationToken));
        void Dispose(bool disposing);
        void Dispose();
    }

    public interface IUnitOfWorkForService
    {
        IGenericRepository<T> Repository<T>() where T : class, new();

    }
}
