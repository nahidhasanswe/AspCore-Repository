using AspNetCore.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.UnitOfWork
{
    public interface IUnitOfWork : IUnitOfWorkForService
    {
        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Setting the flag of multiple table data saves success or not</param>
        /// <returns>Return the number of effected record numbers</returns>
        int Save(bool acceptAllChangesOnSuccess);
        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <returns>Return the number of effected record numbers</returns>
        int Save();
        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Setting the flag of multiple table data saves success or not</param>
        /// <param name="cancellationToken">Cancellation token, defualt is set</param>
        /// <returns>Return the number of effected record numbers</returns>
        Task<int> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="cancellationToken">Cancellation token, defualt is set</param>
        /// <returns>Return the number of effected record numbers</returns>
        Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        /// <param name="disposing">Flag for indicating desposing or not</param>
        void Dispose(bool disposing);
        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        void Dispose();
    }

    public interface IUnitOfWorkForService
    {
        /// <summary>
        /// Get the repository of T Entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns></returns>
        IRepository<T> Repository<T>() where T : class, new();

    }
}
