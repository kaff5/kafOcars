using Microsoft.EntityFrameworkCore;

namespace CoreLib.Db;

public interface IDbContextFactory<out TContext> where TContext : DbContext
{
    TContext CreateWriteContext();
    TContext CreateReadContext();
}