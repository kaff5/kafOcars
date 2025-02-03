using Microsoft.EntityFrameworkCore;
using KafOCars.Domain.Entities;

namespace KafOCars.DataAccess.Contexts
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceRole> ResourceRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.IsActive).IsRequired();

                entity.HasOne(x => x.RegionType).WithMany().HasForeignKey(x => x.RegionTypeId);
            });

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired();
            });

            // UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Resource
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired();
                entity.Property(r => r.Description);
                entity.Property(r => r.Url).IsRequired();
                entity.Property(r => r.HttpMethod).IsRequired();
            });

            // ResourceRole
            modelBuilder.Entity<ResourceRole>(entity =>
            {
                entity.HasKey(rr => new { rr.RoleId, rr.ResourceId });
                entity.HasOne(rr => rr.Role)
                      .WithMany(r => r.RoleResources)
                      .HasForeignKey(rr => rr.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rr => rr.Resource)
                      .WithMany(r => r.RoleResources)
                      .HasForeignKey(rr => rr.ResourceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.ExpiresAt).IsRequired();
                entity.Property(rt => rt.CreatedAt).IsRequired();
                entity.Property(rt => rt.IsRevoked).IsRequired();
                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
