namespace Basionix.BaseEntities.EntityFramework.Interceptors.Tests.TestEntities
{
    using Interceptors.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class SoftDeletableEntityDbContext : DbContext
    {
        public SoftDeletableEntityDbContext(DbContextOptions<SoftDeletableEntityDbContext> options)
            : base(options)
        {
        }
        
        public SoftDeletableEntityDbContext()
        {
        }
        
        public static SoftDeletableEntityDbContext CreateDbContextWithInterceptors(IEnumerable<IInterceptor> interceptors)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SoftDeletableEntityDbContext>();
            var dbName = GenerateDbInstanceName();
            optionsBuilder.UseInMemoryDatabase(dbName);

            if (interceptors != null)
            {
                optionsBuilder.AddInterceptors(interceptors);
            }

            return new SoftDeletableEntityDbContext(optionsBuilder.Options);
        }
        
        public DbSet<SoftDeletableEntity> SoftDeletableEntities { get; set; }

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
        
        private static string GenerateDbInstanceName()
        {
            return $"{nameof(SoftDeletableEntityDbContext)}-{Guid.NewGuid()}-{Random.Shared.Next(1, int.MaxValue)}";
        }
    }
}
