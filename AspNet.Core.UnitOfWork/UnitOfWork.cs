using AspNetCore.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AspNetCore.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork, IUnitOfWork<TContext>, IDisposable where TContext : DbContext
    {

        private readonly TContext _context;

        private bool _disposed = false;

        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public TContext DbContext => _context;

        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        /// <param name="disposing">Flag for indicating desposing or not</param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _context.Dispose();
                    if (_repositories != null)
                    {
                        _repositories.Clear();
                    }
                }

            _disposed = true;
        }

        /// <summary>
        /// Dispose the dbContext after finish task
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the repository of T Entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns></returns>
        public virtual IRepository<T> Repository<T>() where T : class, new()
        {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object>();

            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_context);
            }

            return (IRepository<T>)_repositories[type];
        }

        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Setting the flag of multiple table data saves success or not</param>
        /// <returns>Return the number of effected record numbers</returns>
        public virtual int Save(bool acceptAllChangesOnSuccess)
        {
            return _context.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <returns>Return the number of effected record numbers</returns>
        public virtual int Save()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Setting the flag of multiple table data saves success or not</param>
        /// <param name="cancellationToken">Cancellation token, defualt is set</param>
        /// <returns>Return the number of effected record numbers</returns>
        public virtual async Task<int> SaveAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Save to the database of the change of dbContext
        /// </summary>
        /// <param name="cancellationToken">Cancellation token, defualt is set</param>
        /// <returns>Return the number of effected record numbers</returns>
        public virtual async Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Saves all changes made in this context to the database with distributed transaction.
        /// </summary>
        /// <param name="ensureAutoHistory"><c>True</c> if save changes ensure auto record the change history.</param>
        /// <param name="unitOfWorks">An optional <see cref="IUnitOfWork"/> array.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous save operation. The task result contains the number of state entities written to database.</returns>
        public virtual async Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks)
        {
            using (var ts = new TransactionScope())
            {
                var count = 0;
                foreach (var unitOfWork in unitOfWorks)
                {
                    count += await unitOfWork.SaveAsync(ensureAutoHistory);
                }

                count += await SaveChangesAsync(ensureAutoHistory);

                ts.Complete();

                return count;
            }
        }
    }
}
