using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace AspNet.Core {
    public class UnitOfWork : IUnitOfWork, IDisposable {

        private readonly DbContext _context;
        private readonly Guid _instanceId;

        private bool _disposed = false;
        private Hashtable _repositories;

        public UnitOfWork (DbContext context) {
            _context = context;
            _instanceId = Guid.NewGuid ();
        }

        public Guid InstanceId {
            get { return _instanceId; }
        }

        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        public async Task<int> Save (CancellationToken cancellationToken = default (CancellationToken)) {

            return await _context.SaveChangesAsync (cancellationToken);
        }

        public IGenericRepository<T> Repository<T> () where T : class, new () {
            if (_repositories == null)
                _repositories = new Hashtable ();

            var type = typeof (T).Name;

            if (_repositories.ContainsKey (type))
                return (IGenericRepository<T>) _repositories[type];

            var repositoryType = typeof (IGenericRepository<>);
            if (repositoryType.IsGenericType) {

            }
            object[] paramsArray = new object[] { _context };
            var instance = Activator.CreateInstance (typeof (GenericRepository<T>), paramsArray);
            _repositories.Add (type, instance);

            return (IGenericRepository<T>) _repositories[type];
        }

        public virtual void Dispose (bool disposing) {
            if (!_disposed)
                if (disposing)
                    _context.Dispose ();

            _disposed = true;
        }

        public int Save (bool acceptAllChangesOnSuccess) {
            return _context.SaveChanges (acceptAllChangesOnSuccess);
        }

        public int Save () {
            return _context.SaveChanges ();
        }

        public Task<int> Save (bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default (CancellationToken)) {
            return _context.SaveChangesAsync (acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}