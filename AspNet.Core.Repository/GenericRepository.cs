using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AspNet.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNet.Core.Repository {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        private readonly DbSet<TEntity> dbSet;
        private readonly DbContext _context;

        public GenericRepository (DbContext context) {
            dbSet = context.Set<TEntity> ();
            _context = context;
        }

        public TEntity Add (TEntity entity) {
            dbSet.Add (entity);
            return entity;
        }

        public async Task<TEntity> AddAsync (TEntity entity) {
            await dbSet.AddAsync (entity);
            return entity;
        }

        public void AddRange (params TEntity[] entites) {
            dbSet.AddRange (entites);
        }

        public async Task AddRangeAsync (params TEntity[] entities) {
            await dbSet.AddRangeAsync (entities);
        }

        public TEntity Delete (object key) {
            TEntity entity = Find (key, true);
            dbSet.Remove (entity);
            return entity;
        }

        public TEntity Delete (Expression<Func<TEntity, bool>> predicate) {
            TEntity entity = Find (predicate, true);
            if (entity == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (entity));
            }
            dbSet.Remove (entity);
            return entity;
        }

        public TEntity Delete (TEntity entity) {
            dbSet.Remove (entity);
            return entity;
        }

        public async Task<TEntity> DeleteAsync (object key) {
            TEntity entity = await FindAsync (key, true);
            if (entity == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (entity));
            }
            dbSet.Remove (entity);

            return entity;
        }

        public async Task<TEntity> DeleteAsync (Expression<Func<TEntity, bool>> predicate) {
            TEntity entity = await FindAsync (predicate, true);
            if (entity == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (entity));
            }
            dbSet.Remove (entity);

            return entity;
        }

        public async Task<TEntity> DeleteAsync (TEntity entity) {
            dbSet.Remove (entity);
            return await Task.FromResult<TEntity> (entity);
        }

        public void DeleteRange (Expression<Func<TEntity, bool>> predicate) {
            IEnumerable<TEntity> entities = Get (predicate, null,true);
            foreach (TEntity entity in entities) {
                Delete (entity);
            }

        }

        public void DeleteRange (params TEntity[] entities) {
            foreach (TEntity entity in entities) {
                Delete (entity);
            }
        }

        public async Task DeleteRangeAsync (Expression<Func<TEntity, bool>> predicate) {
            IEnumerable<TEntity> entities = await GetAsync (predicate, null, true);
            if (entities.Count () == 0) {
                ExceptionsBuilder.ThrowNotFoundException ();
            }
            dbSet.RemoveRange (entities);
        }

        public async Task DeleteRangeAsync (params TEntity[] entities) {
            dbSet.RemoveRange (entities);
            await Task.CompletedTask;
        }

        public TEntity Find (object key, bool Tracking = false) {
            if (Tracking) {
                return dbSet.Find (key);
            }

            return dbSet.Find (key);
        }

        public TEntity Find (Expression<Func<TEntity, bool>> predicate, bool Tracking = false) {
            if (Tracking) {
                return dbSet.Find (predicate);
            }

            return dbSet.AsNoTracking ().FirstOrDefault (predicate);
        }

        // public TResult Find<TResult>(object key, Expression<Func<TEntity, TResult>> projection) where TResult : class
        // {
        //     return dbSet.Find(key);
        // }

        public TResult Find<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection) where TResult : class {
            return Where (predicate).Select (projection).FirstOrDefault ();
        }

        public async Task<TEntity> FindAsync (object key, bool Tracking = false) {
            return await dbSet.FindAsync (key);
        }

        public async Task<TEntity> FindAsync (Expression<Func<TEntity, bool>> predicate, bool Tracking = false) {
            if (Tracking) {
                return await dbSet.FindAsync (predicate);
            }

            return await dbSet.AsNoTracking ().FirstOrDefaultAsync (predicate);
        }

        // public Task<TResult> FindAsync<TResult>(object key, Expression<Func<TEntity, TResult>> projection) where TResult : class
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<TResult> FindAsync<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection) where TResult : class {
            return await Where (predicate).Select (projection).FirstOrDefaultAsync ();
        }

        public IEnumerable<TEntity> Get (Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool Tracking = false) {
            
            IQueryable<TEntity> query =  Where(predicate);

            if (orderBy != null) {
                query = orderBy(query);
            }
            return Tracking ? query : query.AsNoTracking ().AsEnumerable ();
        }

        public IEnumerable<TResult> Get<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class {
            
            IQueryable<TEntity> query =  Where(predicate);

            if (orderBy != null) {
                query = orderBy(query);
            }
            
            return query .Select (projection).AsEnumerable ();
        }

        public IEnumerable<TEntity> Get (Expression<Func<TEntity, bool>> predicate, int pageNo, int pageSize, out int totalCount , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) {
            IQueryable<TEntity> query = Where (predicate).AsNoTracking ();

            totalCount = query.Count ();

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.Skip ((pageNo - 1) * pageSize).Take (pageSize).AsEnumerable ();
        }

        public IEnumerable<TResult> Get<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, int pageNo, int pageSize, out int totalCount, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class {
            IQueryable<TEntity> query = Where (predicate);

            totalCount = query.Count ();

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.Skip ((pageNo - 1) * pageSize).Take (pageSize).Select (projection).AsEnumerable ();
        }

        public IEnumerable<TEntity> Get (Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) {
            IQueryable<TEntity> query = Where (predicate);

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.AsNoTracking ().AsEnumerable ();
        }

        public IEnumerable<TResult> Get<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class {
            IQueryable<TEntity> query = Where (predicate);

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.AsNoTracking ().Select (projection).ToList ();
        }

        public IEnumerable<TEntity> Get (Expression<Func<TEntity, bool>> predicate, int pageNo, int pageSize, out int totalCount, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) {
            IQueryable<TEntity> query = Where (predicate);

            totalCount = query.Count ();

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.Skip ((pageNo - 1) * pageSize).Take (pageSize).AsEnumerable ();
        }

        public IEnumerable<TResult> Get<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, int pageNo, int pageSize, out int totalCount, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class {
            IQueryable<TEntity> query = Where (predicate);

            totalCount = query.Count ();

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return query.Skip ((pageNo - 1) * pageSize).Take (pageSize).Select (projection).AsEnumerable ();
        }

        public async Task<IEnumerable<TEntity>> GetAsync (Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool Tracking = false) {

            IQueryable<TEntity> query =  Where(predicate);

            if (orderBy != null) {
                query = orderBy(query);
            }
            return Tracking ? await query.ToListAsync() : await query.AsNoTracking ().ToListAsync ();
        }

        public async Task<IEnumerable<TResult>> GetAsync<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class {
            
            IQueryable<TEntity> query =  Where(predicate);

            if (orderBy != null) {
                query = orderBy(query);
            }
            
            return await query.Select (projection).ToListAsync ();
        }

        public async Task < (int totalCount, IEnumerable<TEntity> entities) > GetAsync (Expression<Func<TEntity, bool>> predicate, int pageNo, int pageSize , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) {
            IQueryable<TEntity> query = Where (predicate).AsNoTracking ();

            int totalCount = await query.CountAsync ();

            if (orderBy != null) {
                query = orderBy(query);
            }

            return (totalCount, await query.Skip ((pageNo - 1) * pageSize).Take (pageSize).ToListAsync ());
        }

        public async Task < (int totalCount, IEnumerable<TResult> entities) > GetAsync<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, int pageNo, int pageSize , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class {
            IQueryable<TEntity> query = Where (predicate).AsNoTracking ();

            int totalCount = await query.CountAsync ();

            if (orderBy != null) {
                query = orderBy(query);
            }

            return (totalCount, await query.Skip ((pageNo - 1) * pageSize).Take (pageSize).Select (projection).ToListAsync ());
        }

        public async Task<IEnumerable<TEntity>> GetAsync (Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) {
            IQueryable<TEntity> query = Where (predicate);

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return await query.AsNoTracking ().ToListAsync ();
        }

        public async Task<IEnumerable<TResult>> GetAsync<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class {
            IQueryable<TEntity> query = Where (predicate);

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return await query.Select (projection).ToListAsync ();
        }

        public async Task < (int totalCount, IEnumerable<TEntity> entities) > GetAsync (Expression<Func<TEntity, bool>> predicate, int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) {
            IQueryable<TEntity> query = Where (predicate);

            int totalCount = await query.CountAsync ();

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return (totalCount, await query.Skip ((pageNo - 1) * pageSize).Take (pageSize).ToListAsync ());
        }

        public async Task < (int totalCount, IEnumerable<TResult> entities) > GetAsync<TResult> (Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class {
            IQueryable<TEntity> query = Where (predicate);

            int totalCount = await query.CountAsync ();

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            if (orderBy != null) {
                query = orderBy(query);
            }

            return (totalCount, await query.Skip ((pageNo - 1) * pageSize).Take (pageSize).Select (projection).ToListAsync ());
        }

        public IQueryable<TEntity> Include (params Expression<Func<TEntity, object>>[] includeProperties) {
            IQueryable<TEntity> query = Where (p => true);

            foreach (Expression<Func<TEntity, object>> entity in includeProperties) {
                query = query.Include<TEntity, object> (entity);
            }

            return query;
        }

        // public IQueryable<TEntity> Include(params object[] includeProperties)
        // {
        //     throw new NotImplementedException();
        // }

        public TEntity Update (TEntity entity, params object[] avoidProperties) {
            return AvoidProperty (entity, avoidProperties.ToList ());
        }

        public async Task<TEntity> UpdateAsync (TEntity entity, params object[] avoidProperties) {
            return await AvoidPropertyAsync (entity, avoidProperties.ToList ());
        }

        public TEntity Update (TEntity entity) {
            dbSet.Update (entity);
            return entity;
        }

        public TEntity Update (object key, TEntity entity) {
            TEntity exist = Find (key, true);

            if (exist == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (exist));
            }

            _context.Entry (exist).CurrentValues.SetValues (entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync (TEntity entity) {
            dbSet.Update (entity);
            return await Task.FromResult<TEntity> (entity);
        }

        public async Task<TEntity> UpdateAsync (object key, TEntity entity) {
            TEntity exist = await FindAsync (key, true);

            if (exist == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (exist));
            }

            _context.Entry (exist).CurrentValues.SetValues (entity);
            return entity;
        }

        public void UpdateRange (params TEntity[] entities) {
            dbSet.UpdateRange (entities);
        }

        public void UpdateRange (List<TEntity> entities, List<object> avoidProperties) {
            foreach (TEntity entity in entities) {
                AvoidProperty (entity, avoidProperties);
            }
        }

        public async Task UpdateRangeAsync (params TEntity[] entities) {
            dbSet.UpdateRange (entities);
            await Task.CompletedTask;
        }

        public async Task UpdateRangeAsync (List<TEntity> entities, List<object> avoidProperties) {
            foreach (TEntity entity in entities) {
                await AvoidPropertyAsync (entity, avoidProperties);
            }
        }

        public TEntity Update (object key, TEntity entity, params object[] avoidProperties) {
            TEntity exist = Find (key, true);

            if (exist == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (exist));
            }

            _context.Entry (exist).CurrentValues.SetValues (entity);
            return AvoidProperty (exist, avoidProperties.ToList ());
        }

        public async Task<TEntity> UpdateAsync (object key, TEntity entity, params object[] avoidProperties) {
            TEntity exist = await FindAsync (key, true);

            if (exist == null) {
                ExceptionsBuilder.ThrowNotFoundException (nameof (exist));
            }

            _context.Entry (exist).CurrentValues.SetValues (entity);
            return await AvoidPropertyAsync (exist, avoidProperties.ToList ());
        }

        public IQueryable<TEntity> Where (Expression<Func<TEntity, bool>> predicate) {
            return dbSet.Where (predicate);
        }

        private TEntity AvoidProperty (TEntity entity, List<object> avoidProperties) {
            EntityEntry entry = dbSet.Update (entity);

            if (avoidProperties != null) {
                foreach (var item in avoidProperties) {
                    foreach (PropertyInfo p in item.GetType ().GetProperties ())
                        entry.Property (p.Name).IsModified = false;
                }
            }

            return entity;
        }

        private async Task<TEntity> AvoidPropertyAsync (TEntity entity, List<object> avoidProperties) {
            EntityEntry entry = dbSet.Update (entity);

            if (avoidProperties != null) {
                foreach (var item in avoidProperties) {
                    foreach (PropertyInfo p in item.GetType ().GetProperties ())
                        entry.Property (p.Name).IsModified = false;
                }
            }

            return await Task.FromResult<TEntity> (entity);
        }
    }
}