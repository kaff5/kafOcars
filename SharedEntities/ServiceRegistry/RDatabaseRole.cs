namespace SharedEntities.ServiceRegistry;

public class RDatabaseRole
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<DatabaseInstance> DatabaseInstances { get; set; } = new List<DatabaseInstance>();
}