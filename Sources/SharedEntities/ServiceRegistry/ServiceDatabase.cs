namespace SharedEntities.ServiceRegistry;

public class ServiceDatabase
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int DatabaseId { get; set; }
    public bool IsPrimary { get; set; } = false;

    public Service Service { get; set; } = null!;
    public Database Database { get; set; } = null!;
}
