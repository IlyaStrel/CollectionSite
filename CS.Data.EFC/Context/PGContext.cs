using CS.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace CS.Data.EFC.Context
{
    public class PGContext : DbContext
    {
        public DbSet<Host> Hosts { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<Trace> Traces { get; set; }

        public PGContext(
            DbContextOptions<PGContext> options
            ) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Host>(entity =>
            {
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            modelBuilder.Entity<Port>(entity =>
            {
                entity.HasIndex(e => new { e.HostId, e.Number });
            });
            modelBuilder.Entity<Trace>(entity =>
            {
                entity.HasIndex(e => new { e.HostId, e.Ip });
            });
        }
    }
}