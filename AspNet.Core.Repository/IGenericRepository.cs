using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNet.Core.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity);
        void AddRange(params TEntity[] entities);
        Task AddRangeAsync(params TEntity[] entities);
        TEntity Update(TEntity entity);
        TEntity Update(object key, TEntity entity);
        TEntity Update(TEntity entity, params object[] avoidProperties);
        TEntity Update(object key, TEntity entity, params object[] avoidProperties);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(object key, TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, params object[] avoidProperties);
        Task<TEntity> UpdateAsync(object key, TEntity entity, params object[] avoidProperties);
        void UpdateRange(params TEntity[] entities);
        void UpdateRange(List<TEntity> entities, List<object> avoidProperties);
        Task UpdateRangeAsync(params TEntity[] entities);
        Task UpdateRangeAsync(List<TEntity> entities, List<object> avoidProperties);

        TEntity Delete(object key);
        Task<TEntity> DeleteAsync(object key);
        TEntity Delete(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Delete(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        void DeleteRange(Expression<Func<TEntity, bool>> predicate);
        Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

        void DeleteRange(params TEntity[] entities);
        Task DeleteRangeAsync(params TEntity[] entities);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties);
        // IQueryable<TEntity> Include(params object[] includeProperties);
        TEntity Find(object key, bool Tracking = false);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, bool Tracking = false);
        // TResult Find<TResult>(object key, Expression<Func<TEntity, TResult>> projection) where TResult : class;
        TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection) where TResult : class;
        Task<TEntity> FindAsync(object key, bool Tracking = false);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate , bool Tracking = false);
        // Task<TResult> FindAsync<TResult>(object key, Expression<Func<TEntity, TResult>> projection) where TResult : class;
        Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection) where TResult : class;
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool Tracking = false);
        IEnumerable<TResult> Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class;
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate,int pageNo, int pageSize, out int totalCount,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        IEnumerable<TResult> Get<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection,int pageNo, int pageSize, out int totalCount, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class;
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TResult> Get<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class;
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate,int pageNo, int pageSize, out int totalCount,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TResult> Get<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection,int pageNo, int pageSize, out int totalCount,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class;

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool Tracking = false);
        Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> projection , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class;
        Task<(int totalCount,IEnumerable<TEntity> entities)> GetAsync(Expression<Func<TEntity, bool>> predicate,int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        Task<(int totalCount,IEnumerable<TResult> entities)> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection,int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null) where TResult : class;
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class;
        Task<(int totalCount,IEnumerable<TEntity> entities)> GetAsync(Expression<Func<TEntity, bool>> predicate,int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<(int totalCount,IEnumerable<TResult> entities)> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> projection,int pageNo, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties) where TResult : class;
    }
}
