namespace SharedEntities.ServiceRegistry;

public class RReplicationStrategy
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // Название стратегии (например, Sharding, Master-Slave)

    public ICollection<Database> Databases { get; set; } = new List<Database>();
}