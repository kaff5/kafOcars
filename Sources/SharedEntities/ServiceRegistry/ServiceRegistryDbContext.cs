using Microsoft.EntityFrameworkCore;

namespace SharedEntities.ServiceRegistry;

public class ServiceRegistryDbContext: DbContext
{
    public ServiceRegistryDbContext(DbContextOptions<ServiceRegistryDbContext> options) : base(options) { }

    public DbSet<RServiceType> ServiceTypes { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<RStatusService> ServiceInstanceStatuses { get; set; } = null!;
    public DbSet<RRegionType> Regions { get; set; } = null!;
    public DbSet<ServiceInstance> ServiceInstances { get; set; } = null!;
    public DbSet<RDatabaseType> DatabaseTypes { get; set; } = null!;
    public DbSet<Database> Databases { get; set; } = null!;
    public DbSet<ServiceDatabase> ServiceDatabases { get; set; } = null!;
    public DbSet<DatabaseInstance> DatabaseInstances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RServiceType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
        
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.RServiceType)
                  .WithMany(e => e.Services)
                  .HasForeignKey(e => e.RServiceType);
        });
        
        modelBuilder.Entity<RStatusService>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
        
        modelBuilder.Entity<RRegionType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired();
        });

        // Configuration for ServiceInstance
        modelBuilder.Entity<ServiceInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Host).IsRequired();
            entity.Property(e => e.Port).IsRequired();
            entity.Property(e => e.LastHeartbeat).HasDefaultValueSql("NOW()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(e => e.Service)
                  .WithMany(e => e.Instances)
                  .HasForeignKey(e => e.ServiceId);
            entity.HasOne(e => e.Region)
                  .WithMany(e => e.ServiceInstances)
                  .HasForeignKey(e => e.RegionId)
                  .IsRequired(false);
            entity.HasOne(e => e.Status)
                  .WithMany(e => e.ServiceInstances)
                  .HasForeignKey(e => e.StatusId);
            
            entity.HasIndex(e => e.ServiceId);
            entity.HasIndex(e => e.RegionId);
            entity.HasIndex(e => e.StatusId);
        });

        // Configuration for DatabaseType
        modelBuilder.Entity<RDatabaseType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
        
        modelBuilder.Entity<RDatabaseRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
        
        modelBuilder.Entity<DatabaseInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Host).IsRequired();
            entity.Property(e => e.Port).IsRequired();
            entity.Property(e => e.ReadWeight).HasDefaultValue(0);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.DatabaseInstances)
                .HasForeignKey(e => e.RoleId);

            entity.HasOne(e => e.Database)
                .WithMany(d => d.DatabaseInstances)
                .HasForeignKey(e => e.DatabaseId);

            entity.HasOne(e => e.Region)
                .WithMany(r => r.DatabaseInstances)
                .HasForeignKey(e => e.RegionId)
                .IsRequired(false);

            entity.HasOne(e => e.Status)
                .WithMany(s => s.DatabaseInstances)
                .HasForeignKey(e => e.StatusId);
        });
        
        modelBuilder.Entity<RReplicationStrategy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
        
        modelBuilder.Entity<Database>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DatabaseKey).IsRequired();
            entity.Property(e => e.ConnectionString).IsRequired();
            entity.Property(e => e.Description);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(e => e.DatabaseType)
                  .WithMany(e => e.Databases)
                  .HasForeignKey(e => e.DatabaseTypeId);
            
            entity.HasIndex(e => e.DatabaseKey).IsUnique();
            entity.HasIndex(e => e.DatabaseTypeId);
            
            entity.HasOne(e => e.ReplicationStrategy)
                .WithMany(r => r.Databases)
                .HasForeignKey(e => e.ReplicationStrategyId);
        });
        
        modelBuilder.Entity<ServiceDatabase>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IsPrimary).HasDefaultValue(false);
            entity.HasOne(e => e.Service)
                  .WithMany(e => e.ServiceDatabases)
                  .HasForeignKey(e => e.ServiceId);
            entity.HasOne(e => e.Database)
                  .WithMany(e => e.ServiceDatabases)
                  .HasForeignKey(e => e.DatabaseId);
        });
        
        modelBuilder.Entity<DatabaseInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Host).IsRequired();
            entity.Property(e => e.Port).IsRequired();
            entity.HasOne(e => e.Database)
                  .WithMany(e => e.DatabaseInstances)
                  .HasForeignKey(e => e.DatabaseId);
            entity.HasOne(e => e.Region)
                  .WithMany(e => e.DatabaseInstances)
                  .HasForeignKey(e => e.RegionId)
                  .IsRequired(false);
            entity.HasOne(e => e.Status)
                  .WithMany(e => e.DatabaseInstances)
                  .HasForeignKey(e => e.StatusId);
            
            
            entity.HasIndex(e => e.DatabaseId);
            entity.HasIndex(e => e.RegionId);
            entity.HasIndex(e => e.StatusId);
        });
    }
}