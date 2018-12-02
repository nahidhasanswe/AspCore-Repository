using Microsoft.EntityFrameworkCore;

namespace AspNetCore.UnitOfWork
{
    public interface IQueryExecutor<TContext> : IQueryExecutor where TContext : DbContext
    {

    }
}
