using CS.Model.DB;
using Microsoft.EntityFrameworkCore;

namespace CS.Data.EFC.Context;

public class SQLiteContext : DbContext
{
    public DbSet<Consumer> Consumers { get; set; }

    public SQLiteContext(
        DbContextOptions<SQLiteContext> options) 
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}