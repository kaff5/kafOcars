using CoreLib.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedEntities.ServiceRegistry;

namespace CoreLib.Infrastucture;

public class ShardResolver
{
    private readonly ServiceRegistryDbContext _dbContext;
    private readonly ServiceSettings _serviceSettings;

    public ShardResolver(ServiceRegistryDbContext dbContext, IOptions<ServiceSettings> serviceSettings)
    {
        _dbContext = dbContext;
        _serviceSettings = serviceSettings.Value;
    }

    /// <summary>
    /// Определяет строку подключения для базы данных, которая обслуживает данный сервис.
    /// </summary>
    /// <returns>Строка подключения к шардированной базе данных.</returns>
    public string ResolveShardConnectionString()
    {
        var serviceType = _serviceSettings.Type;
        var regionName = _serviceSettings.Region;

        if (string.IsNullOrEmpty(serviceType) || string.IsNullOrEmpty(regionName))
        {
            throw new InvalidOperationException("Service type or region is not configured.");
        }

        // Ищем сервис по типу
        var service = _dbContext.Services
            .Include(s => s.RServiceType)
            .FirstOrDefault(s => s.RServiceType.Name == serviceType);

        if (service == null)
        {
            throw new InvalidOperationException($"Service of type {serviceType} is not registered in the database.");
        }

        // Ищем базу данных с шардовой стратегией, привязанную к сервису
        var serviceDatabase = _dbContext.ServiceDatabases
            .Include(sd => sd.Database)
                .ThenInclude(db => db.ReplicationStrategy)
            .FirstOrDefault(sd => sd.ServiceId == service.Id &&
                                  sd.Database.ReplicationStrategy.Name == "Sharding");

        if (serviceDatabase == null)
        {
            throw new InvalidOperationException($"No shard database found for service type {serviceType}.");
        }

        // Ищем доступный экземпляр базы данных
        var databaseInstance = _dbContext.DatabaseInstances
            .Include(di => di.Database)
            .Include(di => di.Region)
            .Include(di => di.Status)
            .FirstOrDefault(di => di.DatabaseId == serviceDatabase.DatabaseId &&
                                  di.Region.Name == regionName &&
                                  di.StatusId == (int)StatusService.Active);

        if (databaseInstance == null)
        {
            throw new InvalidOperationException($"No available shard instance found for region {regionName} and service type {serviceType}.");
        }

        return databaseInstance.Database.ConnectionString;
    }
}
