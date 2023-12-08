using Microsoft.EntityFrameworkCore;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.DataLayer.Context;

public class ContractMarketPlaceDbContext : DbContext
{
    public DbSet<Contract>? Contracts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        const string connectionString = "server=159.89.117.198; port=3306; database=cmp; user=DevOSHT; password=Snodgr4ss!";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration for the 'Contract' entity.
        modelBuilder.Entity<Contract>().HasNoKey();
    }
}