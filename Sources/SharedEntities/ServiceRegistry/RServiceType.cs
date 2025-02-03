namespace SharedEntities.ServiceRegistry;

public class RServiceType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
