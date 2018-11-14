using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.UnitOfWork
{
    public interface IQueryExecutor
    {
        /// <summary>
        /// Execute Sql Command by providing sql parameters
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Number of record affetected</returns>
        int ExecuteSqlCommand(string sql, params object[] parameters);
        /// <summary>
        /// Execute Asynchronous Sql Command by providing sql parameters
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Number of record affetected</returns>
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);
        /// <summary>
        /// Execute the Sql Command of an Entity
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type of DContext</typeparam>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Return the List of Entity</returns>
        List<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class;
        /// <summary>
        /// Execute Async the Sql Command of an Entity
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type of DContext</typeparam>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Return the List of Entity with Task</returns>
        Task<List<TEntity>> FromSqlAsync<TEntity>(string sql, params object[] parameters) where TEntity : class;
        /// <summary>
        /// Execute the Raw Sql Command 
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type Of Return Type</typeparam>
        /// <param name="query">Raw Sql Command </param>
        /// <returns>Return the list of provided Entity Type</returns>
        List<TEntity> ExecSQL<TEntity>(string query);
        /// <summary>
        /// Execute Async the Raw Sql Command 
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type Of Return Type</typeparam>
        /// <param name="query">Raw Sql Command </param>
        /// <returns>Return the list of provided Entity Type with Task</returns>
        Task<List<TEntity>> ExecSQLAsync<TEntity>(string query);
        /// <summary>
        /// Execute the Sql Command with Filter of Where Condition
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <returns>Return the list of provided Entity Type</returns>
        List<TEntity> ExecFilter<TEntity>(string filter);
        /// <summary>
        /// Execute Async the Sql Command with Filter of Where Condition
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <returns>Return the list of provided Entity Type with Task</returns>
        Task<List<TEntity>> ExecFilterAsync<TEntity>(string filter);
        /// <summary>
        /// Execute the Sql Command with Filter of Where Condition and Get Custom Entity List
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <typeparam name="TResult">provide the return Entity Type</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <param name="selector">Provide the Selector Between TEntity and TResult</param>
        /// <param name="parameters">Provide SqlParameter List of filter query</param>
        /// <returns></returns>
        List<TResult> ExecFilter<TEntity,TResult>(string filter,Expression<Func<TEntity,TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class;
        /// <summary>
        /// Execute Async the Sql Command with Filter of Where Condition and Get Custom Entity List
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <typeparam name="TResult">provide the return Entity Type</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <param name="selector">Provide the Selector Between TEntity and TResult</param>
        /// <param name="parameters">Provide SqlParameter List of filter query</param>
        Task<List<TResult>> ExecFilterAsync<TEntity,TResult>(string filter,Expression<Func<TEntity,TResult>> selector, params SqlParameter[] parameters) where TEntity : class where TResult : class;
        /// <summary>
        /// Execute Scalar Sql Command
        /// </summary>
        /// <param name="query">Sql Query</param>
        /// <returns>Return the object of Scalar object query</returns>
        object ExecScalar(string query);
        /// <summary>
        /// Execute Scalar Sql Command
        /// </summary>
        /// <param name="query">Sql Query</param>
        /// <param name="parameters">Provide SqlParameter List of filter query</param>
        /// <returns>Return the object of Scalar object query</returns>
        object ExecScalar(string query, params SqlParameter[] parameters);
    }
}