namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests.TestEntities
{
    using Interceptors.Extensions;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class AuditableEntityDbContext : DbContext
    {
        public AuditableEntityDbContext(DbContextOptions<AuditableEntityDbContext> options)
            : base(options)
        {
        }

        public AuditableEntityDbContext()
        {
        }
        
        public static AuditableEntityDbContext CreateDbContextWithInterceptors(IEnumerable<IInterceptor> interceptors)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuditableEntityDbContext>();
            var dbName = GenerateDbInstanceName();
            optionsBuilder.UseInMemoryDatabase(dbName);

            if (interceptors != null)
            {
                optionsBuilder.AddInterceptors(interceptors);
            }

            return new AuditableEntityDbContext(optionsBuilder.Options);
        }
        
        public DbSet<AuditableEntity> AuditableEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase(GenerateDbInstanceName());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<AuditableEntity>(e =>
                {
                    e.HasKey(p => p.Id);
                    e.ConfigureAuditProperties();
                    e.HasData([
                        new AuditableEntity{ Id = 1, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user", LastUpdatedBy = null, UpdatedAt = null },
                        new AuditableEntity{ Id = 2, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user", LastUpdatedBy = null, UpdatedAt = null },
                        new AuditableEntity{ Id = 3, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user", LastUpdatedBy = null, UpdatedAt = null },
                        new AuditableEntity{ Id = 4, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user", LastUpdatedBy = null, UpdatedAt = null },
                        new AuditableEntity{ Id = 5, CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "test_user", LastUpdatedBy = null, UpdatedAt = null },
                    ]);
                });
        }

        private static string GenerateDbInstanceName()
        {
            return $"{nameof(AuditableEntityDbContext)}-{Guid.NewGuid()}-{Random.Shared.Next(1, int.MaxValue)}";
        }
    }
}
