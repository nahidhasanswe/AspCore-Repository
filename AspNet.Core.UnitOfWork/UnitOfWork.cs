using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AspNet.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.UnitOfWork {
    public class UnitOfWork<TContext> : IUnitOfWork, IUnitOfWork<TContext>, IDisposable where TContext : DbContext {

        private readonly TContext _context;
        private readonly Guid _instanceId;

        private bool _disposed = false;
        private Dictionary<Type, object> _repositories;

        public UnitOfWork (TContext context) {
            _context = context ??
                throw new ArgumentNullException (nameof (context));;
            _instanceId = Guid.NewGuid ();
        }

        public Guid InstanceId {
            get { return _instanceId; }
        }

        public TContext DbContext => _context;

        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        public async Task<int> SaveAsync (CancellationToken cancellationToken = default (CancellationToken)) {

            return await _context.SaveChangesAsync (cancellationToken);
        }

        public IGenericRepository<T> Repository<T> () where T : class, new () {
            if (_repositories == null)
                _repositories = new Dictionary<Type, object> ();

            var type = typeof (T);

            if (!_repositories.ContainsKey (type)) {
                _repositories[type] = new GenericRepository<T> (_context);
            }

            return (IGenericRepository<T>) _repositories[type];
        }

        public virtual void Dispose (bool disposing) {
            if (!_disposed)
                if (disposing) {
                    _context.Dispose ();
                    if (_repositories != null) {
                        _repositories.Clear ();
                    }
                }

            _disposed = true;
        }

        public int Save (bool acceptAllChangesOnSuccess) {
            return _context.SaveChanges (acceptAllChangesOnSuccess);
        }

        public int Save () {
            return _context.SaveChanges ();
        }

        public Task<int> SaveAsync (bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default (CancellationToken)) {
            return _context.SaveChangesAsync (acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task<int> SaveChangesAsync (bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks) {
            using (var ts = new TransactionScope ()) {
                var count = 0;
                foreach (var unitOfWork in unitOfWorks) {
                    count += await unitOfWork.SaveAsync (ensureAutoHistory);
                }

                count += await SaveChangesAsync (ensureAutoHistory);

                ts.Complete ();

                return count;
            }
        }

        public int ExecuteSqlCommand (string sql, params object[] parameters) => _context.Database.ExecuteSqlCommand (sql, parameters);

        public async Task<int> ExecuteSqlCommandAsync (string sql, params object[] parameters) => await _context.Database.ExecuteSqlCommandAsync (sql, parameters);

        public List<TEntity> FromSql<TEntity> (string sql, params object[] parameters) where TEntity : class {
            return _context.Set<TEntity> ().FromSql (sql, parameters).ToList ();
        }

        public async Task<List<TEntity>> FromSqlAsync<TEntity> (string sql, params object[] parameters) where TEntity : class {
            return await _context.Set<TEntity> ().FromSql (sql, parameters).ToListAsync ();
        }

        public List<TEntity> ExecSQL<TEntity> (string query) {
            try {
                List<TEntity> list = new List<TEntity> ();

                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection ();

                    using (var result = command.ExecuteReader ()) {
                        TEntity obj = default (TEntity);
                        while (result.Read ()) {
                            obj = Activator.CreateInstance<TEntity> ();
                            foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
                                try {
                                    if (!object.Equals (result[prop.Name], DBNull.Value)) {
                                        prop.SetValue (obj, result[prop.Name], null);
                                    }
                                } catch (Exception) { }
                            }
                            list.Add (obj);
                        }
                    }
                }

                return list;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<TEntity>> ExecSQLAsync<TEntity> (string query) {
            try {
                List<TEntity> list = new List<TEntity> ();

                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection ();

                    using (var result = await command.ExecuteReaderAsync ()) {
                        TEntity obj = default (TEntity);
                        while (await result.ReadAsync ()) {
                            obj = Activator.CreateInstance<TEntity> ();
                            foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
                                try {
                                    if (!object.Equals (result[prop.Name], DBNull.Value)) {
                                        prop.SetValue (obj, result[prop.Name], null);
                                    }
                                } catch (Exception) { }
                            }
                            list.Add (obj);
                        }
                    }
                }

                return list;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public List<TEntity> ExecFilter<TEntity> (string filter) {
            try {
                List<TEntity> list = new List<TEntity> ();

                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                    Type type = typeof (TEntity);
                    if (type == null) throw new Exception ("Model not found");
                    string name = _context.Model.FindEntityType (typeof (TEntity)).Relational ().TableName;

                    if (!string.IsNullOrEmpty (filter))
                        filter = "WHERE " + filter.Trim ();

                    command.CommandText = string.Format ("select * from [{0}] {1}", name, filter);
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection ();

                    using (var result = command.ExecuteReader ()) {
                        TEntity obj = default (TEntity);
                        while (result.Read ()) {
                            obj = Activator.CreateInstance<TEntity> ();
                            foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
                                try {
                                    if (!object.Equals (result[prop.Name], DBNull.Value)) {
                                        prop.SetValue (obj, result[prop.Name], null);
                                    }
                                } catch (Exception) { }
                            }
                            list.Add (obj);
                        }
                    }
                }

                return list;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<TEntity>> ExecFilterAsync<TEntity> (string filter) {
            try {
                List<TEntity> list = new List<TEntity> ();

                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                    Type type = typeof (TEntity);
                    if (type == null) throw new Exception ("Model not found");
                    string name = _context.Model.FindEntityType (typeof (TEntity)).Relational ().TableName;

                    if (!string.IsNullOrEmpty (filter))
                        filter = "WHERE " + filter.Trim ();

                    command.CommandText = string.Format ("select * from [{0}] {1}", name, filter);
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection ();

                    using (var result = await command.ExecuteReaderAsync ()) {
                        TEntity obj = default (TEntity);
                        while (await result.ReadAsync ()) {
                            obj = Activator.CreateInstance<TEntity> ();
                            foreach (PropertyInfo prop in obj.GetType ().GetProperties ()) {
                                try {
                                    if (!object.Equals (result[prop.Name], DBNull.Value)) {
                                        prop.SetValue (obj, result[prop.Name], null);
                                    }
                                } catch (Exception) { }
                            }
                            list.Add (obj);
                        }
                    }
                }

                return list;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public List<TResult> ExecFilter<TEntity, TResult> (string filter, Expression<Func<TEntity, TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class {
            Type type = typeof (TEntity);
            if (type == null) throw new Exception ("Model not found");
            string name = _context.Model.FindEntityType (typeof (TEntity)).Relational ().TableName;

            if (!string.IsNullOrEmpty (filter)) {
                filter = string.Format ("SELECT * FROM {0} WHERE {1}", name, filter);
            }

            return FromSqlPrivate<TEntity> (filter, parameters).Select (selector).ToList ();
        }

        public async Task<List<TResult>> ExecFilterAsync<TEntity, TResult> (string filter, Expression<Func<TEntity, TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class {
            Type type = typeof (TEntity);
            if (type == null) throw new Exception ("Model not found");
            string name = _context.Model.FindEntityType (typeof (TEntity)).Relational ().TableName;

            if (!string.IsNullOrEmpty (filter)) {
                filter = string.Format ("SELECT * FROM {0} WHERE {1}", name, filter);
            }

            return await FromSqlPrivate<TEntity> (filter, parameters).Select (selector).ToListAsync ();
        }

        public object ExecScalar (string query) {
            try {
                object obj = new object ();
                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;

                        _context.Database.OpenConnection ();

                        obj = command.ExecuteScalar ();
                    }
                return obj;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public object ExecScalar (string query, params SqlParameter[] parameters) {
            try {
                object obj = new object ();
                using (var command = _context.Database.GetDbConnection ().CreateCommand ()) {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;

                        if(parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        _context.Database.OpenConnection ();

                        obj = command.ExecuteScalar ();
                    }
                return obj;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private IQueryable<TEntity> FromSqlPrivate<TEntity> (string sqlQuery, params SqlParameter[] parameters) where TEntity : class {
            if (parameters != null) {
                if (parameters.Count () > 0) {
                    return _context.Set<TEntity> ().FromSql (sqlQuery, parameters);
                }
            }
            return _context.Set<TEntity> ().FromSql (sqlQuery);
        }
    }
}