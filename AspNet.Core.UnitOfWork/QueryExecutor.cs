using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.UnitOfWork
{
    public class QueryExecutor<TContext> : IQueryExecutor,IQueryExecutor<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public QueryExecutor(TContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }


        /// <summary>
        /// Execute the Sql Command with Filter of Where Condition
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <returns>Return the list of provided Entity Type</returns>
        public virtual List<TEntity> ExecFilter<TEntity>(string filter)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    Type type = typeof(TEntity);
                    if (type == null) throw new Exception("Model not found");
                    string name = _context.Model.FindEntityType(typeof(TEntity)).Relational().TableName;

                    if (!string.IsNullOrEmpty(filter))
                        filter = "WHERE " + filter.Trim();

                    command.CommandText = string.Format("select * from [{0}] {1}", name, filter);
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        TEntity obj = default(TEntity);
                        while (result.Read())
                        {
                            obj = Activator.CreateInstance<TEntity>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                try
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }
                                catch (Exception) { }
                            }
                            list.Add(obj);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Execute the Sql Command with Filter of Where Condition
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <returns>Return the list of provided Entity Type</returns>
        public virtual List<TResult> ExecFilter<TEntity, TResult>(string filter, Expression<Func<TEntity, TResult>> selector, params object[] parameters)
            where TEntity : class
            where TResult : class
        {
            Type type = typeof(TEntity);
            if (type == null) throw new Exception("Model not found");
            string name = _context.Model.FindEntityType(typeof(TEntity)).Relational().TableName;

            if (!string.IsNullOrEmpty(filter))
            {
                filter = string.Format("SELECT * FROM {0} WHERE {1}", name, filter);
            }

            return FromSqlPrivate<TEntity>(filter, parameters).Select(selector).ToList();
        }

        /// <summary>
        /// Execute Async the Sql Command with Filter of Where Condition
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <returns>Return the list of provided Entity Type with Task</returns>
        public virtual async Task<List<TEntity>> ExecFilterAsync<TEntity>(string filter)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    Type type = typeof(TEntity);
                    if (type == null) throw new Exception("Model not found");
                    string name = _context.Model.FindEntityType(typeof(TEntity)).Relational().TableName;

                    if (!string.IsNullOrEmpty(filter))
                        filter = "WHERE " + filter.Trim();

                    command.CommandText = string.Format("select * from [{0}] {1}", name, filter);
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        TEntity obj = default(TEntity);
                        while (await result.ReadAsync())
                        {
                            obj = Activator.CreateInstance<TEntity>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                try
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }
                                catch (Exception) { }
                            }
                            list.Add(obj);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Execute the Sql Command with Filter of Where Condition and Get Custom Entity List
        /// </summary>
        /// <typeparam name="TEntity">provide the type Entity of db Context</typeparam>
        /// <typeparam name="TResult">provide the return Entity Type</typeparam>
        /// <param name="filter">Provide the query after WHERE condition</param>
        /// <param name="selector">Provide the Selector Between TEntity and TResult</param>
        /// <param name="parameters">Provide SqlParameter List of filter query</param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> ExecFilterAsync<TEntity, TResult>(string filter, Expression<Func<TEntity, TResult>> selector, params object[] parameters)
            where TEntity : class
            where TResult : class
        {
            Type type = typeof(TEntity);
            if (type == null) throw new Exception("Model not found");
            string name = _context.Model.FindEntityType(typeof(TEntity)).Relational().TableName;

            if (!string.IsNullOrEmpty(filter))
            {
                filter = string.Format("SELECT * FROM {0} WHERE {1}", name, filter);
            }

            return await FromSqlPrivate<TEntity>(filter, parameters).Select(selector).ToListAsync();
        }


        /// <summary>
        /// Execute Scalar Sql Command
        /// </summary>
        /// <param name="query">Sql Query</param>
        /// <returns>Return the object of Scalar object query</returns>
        public virtual object ExecScalar(string query)
        {
            try
            {
                object obj = new object();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    _context.Database.OpenConnection();

                    obj = command.ExecuteScalar();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Execute Scalar Sql Command
        /// </summary>
        /// <param name="query">Sql Query</param>
        /// <param name="parameters">Provide SqlParameter List of filter query</param>
        /// <returns>Return the object of Scalar object query</returns>
        public virtual object ExecScalar(string query, params object[] parameters)
        {
            try
            {
                object obj = new object();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    if (parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    _context.Database.OpenConnection();

                    obj = command.ExecuteScalar();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Execute the Raw Sql Command 
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type Of Return Type</typeparam>
        /// <param name="query">Raw Sql Command </param>
        /// <returns>Return the list of provided Entity Type</returns>
        public virtual List<TEntity> ExecSQL<TEntity>(string query)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        TEntity obj = default(TEntity);
                        while (result.Read())
                        {
                            obj = Activator.CreateInstance<TEntity>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                try
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }
                                catch (Exception) { }
                            }
                            list.Add(obj);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Execute Async the Raw Sql Command 
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type Of Return Type</typeparam>
        /// <param name="query">Raw Sql Command </param>
        /// <returns>Return the list of provided Entity Type with Task</returns>
        public virtual async Task<List<TEntity>> ExecSQLAsync<TEntity>(string query)
        {
            try
            {
                List<TEntity> list = new List<TEntity>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        TEntity obj = default(TEntity);
                        while (await result.ReadAsync())
                        {
                            obj = Activator.CreateInstance<TEntity>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                try
                                {
                                    if (!object.Equals(result[prop.Name], DBNull.Value))
                                    {
                                        prop.SetValue(obj, result[prop.Name], null);
                                    }
                                }
                                catch (Exception) { }
                            }
                            list.Add(obj);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Execute Sql Command by providing sql parameters
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Number of record affetected</returns>
        public virtual int ExecuteSqlCommand(string sql, params object[] parameters) => _context.Database.ExecuteSqlCommand(sql, parameters);

        /// <summary>
        /// Execute Asynchronous Sql Command by providing sql parameters
        /// </summary>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Number of record affetected</returns>
        public virtual async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters) => await _context.Database.ExecuteSqlCommandAsync(sql, parameters);

        /// <summary>
        /// Execute the Sql Command of an Entity
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type of DContext</typeparam>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Return the List of Entity</returns>
        public virtual List<TEntity> FromSql<TEntity>(string sql, params object[] parameters) where TEntity : class
        {
            return _context.Set<TEntity>().FromSql(sql, parameters).ToList();
        }

        /// <summary>
        /// Execute Async the Sql Command of an Entity
        /// </summary>
        /// <typeparam name="TEntity">Provide the Entity type of DContext</typeparam>
        /// <param name="sql">The sql query</param>
        /// <param name="parameters">Sql query parameters</param>
        /// <returns>Return the List of Entity with Task</returns>
        public virtual async Task<List<TEntity>> FromSqlAsync<TEntity>(string sql, params object[] parameters) where TEntity : class
        {
            return await _context.Set<TEntity>().FromSql(sql, parameters).ToListAsync();
        }

        private IQueryable<TEntity> FromSqlPrivate<TEntity>(string sqlQuery, params object[] parameters) where TEntity : class
        {
            if (parameters != null)
            {
                if (parameters.Count() > 0)
                {
                    return _context.Set<TEntity>().FromSql(sqlQuery, parameters);
                }
            }
            return _context.Set<TEntity>().FromSql(sqlQuery);
        }
    }
}
