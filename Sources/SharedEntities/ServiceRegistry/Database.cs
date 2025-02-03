namespace SharedEntities.ServiceRegistry;

public class Database
{
    public int Id { get; set; }
    public string DatabaseKey { get; set; } = null!; // Уникальный ключ базы данных
    public string ConnectionString { get; set; } = null!; // Строка подключения
    public int DatabaseTypeId { get; set; } // Ссылка на тип базы данных
    public int ReplicationStrategyId { get; set; } // Ссылка на стратегию репликации
    public string? Description { get; set; }
    public bool IsPrimary { get; set; } = true; // Признак основной базы данных
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public RDatabaseType DatabaseType { get; set; } = null!;
    public RReplicationStrategy ReplicationStrategy { get; set; } = null!;
    public ICollection<ServiceDatabase> ServiceDatabases { get; set; } = new List<ServiceDatabase>();
    public ICollection<DatabaseInstance> DatabaseInstances { get; set; } = new List<DatabaseInstance>();
}
