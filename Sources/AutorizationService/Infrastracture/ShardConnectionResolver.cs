using SharedEntities.ServiceRegistry;

namespace KafOCars.Infrastracture;


public interface IShardConnectionResolver
{
    string? ResolveConnectionString(string shardKey);
}

public class ShardConnectionResolver : IShardConnectionResolver
{
    private readonly ServiceRegistryDbContext _dbContext;

    public ShardConnectionResolver(ServiceRegistryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string? ResolveConnectionString(string shardKey)
    {
        var database = _dbContext.Databases.FirstOrDefault(d => d.DatabaseKey == shardKey);
        return database?.ConnectionString;
    }
}