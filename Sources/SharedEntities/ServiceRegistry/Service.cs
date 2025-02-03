namespace SharedEntities.ServiceRegistry;

public class Service
{
    public int Id { get; set; }
    public int ServiceTypeId { get; set; }

    // Навигационные свойства
    public RServiceType RServiceType { get; set; } = null!;
    public ICollection<ServiceInstance> Instances { get; set; } = new List<ServiceInstance>(); // Инстансы сервиса
    public ICollection<ServiceDatabase> ServiceDatabases { get; set; } = new List<ServiceDatabase>(); // Связанные базы
}
