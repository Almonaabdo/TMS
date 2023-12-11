using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.DataLayer.Context;

public class ContractMarketPlaceDbContext : DbContext
{
    public DbSet<Contract>? Contracts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("TMSDB");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration for the 'Contract' entity.
        modelBuilder.Entity<Contract>().HasNoKey();
    }
}