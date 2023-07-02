using Microsoft.EntityFrameworkCore;

namespace CS.Data.PG.Context
{
    public class CSDataContext : DbContext
    {
        public CSDataContext(
            DbContextOptions<CSDataContext> options) : base(options)
        {

        }

        //public DbSet<MeteostationClickhouseData> MeteoData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


        }
    }
}