using Microsoft.EntityFrameworkCore;

namespace MiniDemo.Model
{
    public class AuditableDbContext : DbContext
    {
        public AuditableDbContext()
        {

        }

        public AuditableDbContext(DbContextOptions<AuditableDbContext> options) : base(options)
        {
        }

        public DbSet<AuditTrail> AuditTrails { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("audit");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("AppDb");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
