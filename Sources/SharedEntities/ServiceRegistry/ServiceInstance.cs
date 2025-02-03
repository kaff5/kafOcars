using CoreLib.Enum;
namespace SharedEntities.ServiceRegistry;
public class ServiceInstance
{
    public int Id { get; set; }
    
    /// <summary>
    /// <see cref="ServiceType"/>>
    /// </summary>
    public int ServiceId { get; set; }
    public int? RegionId { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    
    /// <summary>
    /// <see cref="StatusService"/>>
    /// </summary>
    public int StatusId { get; set; } = (int)StatusService.Active;
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow; // Последний сигнал активности
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public Service Service { get; set; } = null!;
    public RStatusService Status { get; set; } = null!;
    public RRegionType? Region { get; set; } // Связь с регионом (NULL = все регионы)
}
