using Microsoft.EntityFrameworkCore;
using TMS_Project.DataLayer.Model;

namespace DataLayer.Context;

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
        base.OnModelCreating(modelBuilder);
        
        // Configuration for the 'Contract' entity.
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contract");
            entity.HasKey(e => e.ContractId); // Set ContractId as primary key
            entity.Property(e => e.ContractId).ValueGeneratedOnAdd();
            entity.Property(e => e.ConctractDetails).IsRequired();
        });
    }
}