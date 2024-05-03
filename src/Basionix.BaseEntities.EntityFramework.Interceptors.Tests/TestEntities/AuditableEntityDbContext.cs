namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests;

using Microsoft.EntityFrameworkCore;

public class AuditableEntityDbContext : DbContext
{
    public AuditableEntityDbContext() : base()
    {
    }

    public AuditableEntityDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AuditableEntity> AuditableEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase($"{nameof(AuditableEntityDbContext)}-{Guid.NewGuid()}-{Random.Shared.Next(1, int.MaxValue)}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<AuditableEntity>()
            .HasKey(p => p.Id);
            
        modelBuilder
            .Entity<AuditableEntity>()
            .ConfigureAuditProperties();
        
        modelBuilder
            .Entity<AuditableEntity>()
            .HasData([
                new AuditableEntity{ Id = 1, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user"},
                new AuditableEntity{ Id = 2, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user"},
                new AuditableEntity{ Id = 3, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user"},
                new AuditableEntity{ Id = 4, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user"},
                new AuditableEntity{ Id = 5, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user"},
            ]);
    }
}
