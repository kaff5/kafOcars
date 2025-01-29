namespace CoreLib.Db;

public interface IDbContextFactory<out TContext> where TContext : class
{
    TContext CreateWriteContext();
    TContext CreateReadContext();
}