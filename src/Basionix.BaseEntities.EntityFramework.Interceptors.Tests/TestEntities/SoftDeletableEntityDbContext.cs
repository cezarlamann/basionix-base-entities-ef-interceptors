namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests;

using Microsoft.EntityFrameworkCore;

public class SoftDeletableEntityDbContext : DbContext
{
    public SoftDeletableEntityDbContext() : base()
    {
    }

    public SoftDeletableEntityDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<SoftDeletableEntity> SoftDeletableEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase($"{nameof(SoftDeletableEntityDbContext)}-{Guid.NewGuid()}-{Random.Shared.Next(1, int.MaxValue)}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<SoftDeletableEntity>()
            .HasKey(p => p.Id);
            
        modelBuilder
            .Entity<SoftDeletableEntity>()
            .ConfigureSoftDeletableProperties();
        
        modelBuilder
            .Entity<SoftDeletableEntity>()
            .HasData([
                new SoftDeletableEntity{ Id = 1 },
                new SoftDeletableEntity{ Id = 2 },
                new SoftDeletableEntity{ Id = 3 },
                new SoftDeletableEntity{ Id = 4 },
                new SoftDeletableEntity{ Id = 5 },
            ]);
    }
}
