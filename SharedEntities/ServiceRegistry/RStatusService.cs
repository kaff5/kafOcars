namespace SharedEntities.ServiceRegistry;

public class RStatusService
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<ServiceInstance> ServiceInstances { get; set; } = new List<ServiceInstance>();
    public ICollection<DatabaseInstance> DatabaseInstances { get; set; } = new List<DatabaseInstance>();
}