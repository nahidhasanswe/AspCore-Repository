using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.UnitOfWork
{
    public interface IQueryExecutor
    {
        int ExecuteSqlCommand(string sql, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        List<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
        Task<List<TEntity>> FromSqlAsync<TEntity>(string sql, params object[] parameters) where TEntity : class;
        List<TEntity> ExecSQL<TEntity>(string query);
        Task<List<TEntity>> ExecSQLAsync<TEntity>(string query);
        List<TEntity> ExecFilter<TEntity>(string filter);
        Task<List<TEntity>> ExecFilterAsync<TEntity>(string filter);
        List<TResult> ExecFilter<TEntity,TResult>(string filter,Expression<Func<TEntity,TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class;
        Task<List<TResult>> ExecFilterAsync<TEntity,TResult>(string filter,Expression<Func<TEntity,TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class;
        object ExecScalar(string query);
        object ExecScalar(string query, params SqlParameter[] parameters);
    }
}