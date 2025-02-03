using CoreLib.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedEntities.ServiceRegistry;

namespace CoreLib.Infrastucture;

public class ServiceInstanceRegistrar
{
    private readonly ServiceRegistryDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public ServiceInstanceRegistrar(ServiceRegistryDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Регистрирует текущий инстанс сервиса в таблице ServiceInstances.
    /// </summary>
    /// <param name="serviceType">Тип сервиса</param>
    public async Task RegisterInstanceAsync(ServiceType serviceType)
    {
        var service = await _dbContext.Services
            .Where(s => s.ServiceTypeId == (int)serviceType)
            .FirstOrDefaultAsync();

        if (service == null)
        {
            throw new InvalidOperationException($"Service type {serviceType} is not registered in the database.");
        }

        var instance = new ServiceInstance
        {
            ServiceId = service.Id,
            Host = _configuration["Service:Host"] ?? "localhost",
            Port = int.Parse(_configuration["Service:Port"] ?? "5000"),
            StatusId = (int)StatusService.Active,
            RegionId = null, // Может быть указано в конфигурации или передано как параметр
            LastHeartbeat = DateTime.UtcNow
        };

        _dbContext.ServiceInstances.Add(instance);
        await _dbContext.SaveChangesAsync();
    }
}
