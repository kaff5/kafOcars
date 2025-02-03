using CoreLib.Db;
using Microsoft.EntityFrameworkCore;

namespace KafOCars.DataAccess.Contexts;

public class PostgresDbContextFactory<TContext> : CoreLib.Db.IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly DatabaseSettings _settings;
    private int _currentReplicaIndex = 0;
    private readonly object _lock = new object();

    public PostgresDbContextFactory(DatabaseSettings settings)
    {
        _settings = settings;
    }

    public TContext CreateWriteContext()
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(_settings.MasterConnection)
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }

    public TContext CreateReadContext()
    {
        //применяем round robin для выбора реплики
        string connection;
        lock (_lock)
        {
            connection = _settings.ReplicaConnections[_currentReplicaIndex];
            _currentReplicaIndex = (_currentReplicaIndex + 1) % _settings.ReplicaConnections.Length;
        }

        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(connection)
            .Options;

        return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
    }
}