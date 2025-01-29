namespace SharedEntities.ServiceRegistry;

public class DatabaseInstance
{
    public int Id { get; set; }
    public int DatabaseId { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public int? RegionId { get; set; }
    public int StatusId { get; set; }
    public int RoleId { get; set; }
    public int ReadWeight { get; set; } = 0; // Вес для распределения нагрузки на чтение
    public bool IsAvailable { get; set; } = true; // Признак доступности

    public Database Database { get; set; } = null!;
    public RRegionType? Region { get; set; }
    public RStatusService Status { get; set; } = null!;
    public RDatabaseRole Role { get; set; } = null!;
}