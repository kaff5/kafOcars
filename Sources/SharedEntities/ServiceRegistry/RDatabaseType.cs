namespace SharedEntities.ServiceRegistry;

public class RDatabaseType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Database> Databases { get; set; } = new List<Database>();
}