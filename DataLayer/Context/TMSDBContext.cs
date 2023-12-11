using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.DataLayer.Context
{
    public class TmsDbContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<City>? Cities { get; set; }
        public DbSet<Carrier>? Carriers { get; set; }
        public DbSet<Trip>? Trips { get; set; }
        public DbSet<Rate>? Rates { get; set; }
        public DbSet<Route>? Routes { get; set; }
        public DbSet<LogFile>? LogFiles { get; set; }
        public DbSet<Invoice>? InvoiceDetails { get;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("RemoteDB");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration for the 'Customer' entity.
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber);
                entity.Property(e => e.Email);

                // Relationship: Each customer is associated with one invoice.
                entity.HasMany(c => c.Invoices).WithOne(i => i.Customer).HasForeignKey(i => i.CustomerId);
                
                
                // Relationship: A customer can have multiple orders
                entity.HasMany(e => e.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // Configuration for the 'Carrier' entity.
            modelBuilder.Entity<Carrier>(entity =>   // DONE
            {
                entity.ToTable("Carrier");
                entity.HasKey(e => e.CarrierId);
                entity.Property(e => e.CarrierId).ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyName).IsRequired();
                entity.Property(e => e.DepotCity).IsRequired();
                entity.Property(e => e.Ftla).IsRequired();
                entity.Property(e => e.Ltla).IsRequired();
                entity.Property(e => e.FtlRate).IsRequired();
                entity.Property(e => e.LtlRate).IsRequired();
                entity.Property(e => e.ReefCharge).IsRequired();

                // Relationship: A Carrier can have multiple Trips
                entity.HasMany(e => e.Trips).WithOne(t => t.Carrier).HasForeignKey(t => t.CarrierId);

            });

            // Configuration for the 'City' entity.
            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");
                entity.HasKey(e => e.CityId);
                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
                entity.Property(e => e.CityName).IsRequired();

                // Configuration for the 'City' entity.
                entity.HasMany(e => e.SourceOrders)
                    .WithOne(o => o.SourceCity)
                    .HasForeignKey(o => o.SourceCityId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.DestinationOrders)
                    .WithOne(o => o.DestinationCity)
                    .HasForeignKey(o => o.DestinationCityId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with route: Each city can be a source or destination for multiple routes
                entity.HasMany(e => e.SourceRoutes)
                    .WithOne(r => r.SourceCity)
                    .HasForeignKey(r => r.SourceCityId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasMany(e => e.DestinationRoutes)
                    .WithOne(r => r.DestinationCity)
                    .HasForeignKey(r => r.DestinationCityId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // Configuration for the 'Invoice' entity.
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");
                entity.HasKey(e => e.InvoiceId);
                entity.Property(e => e.InvoiceId).ValueGeneratedOnAdd();
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.RateId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.InvoiceDate).IsRequired();

                // Relationship: An Invoice belongs to one Order
                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Invoices)
                    .HasForeignKey<Invoice>(e => e.OrderId);

                // Relationship : An Invoice has one Rate
                 entity.HasOne(e => e.Rates)
                     .WithMany()
                     .HasForeignKey(e => e.RateId);

                 // Relationship : An Invoice belongs to one Customer
                 entity.HasOne(e => e.Customer)
                     .WithMany(c => c.Invoices)
                     .HasForeignKey((e => e.CustomerId));
            });

            // Configuration for the 'LogFile' entity.
            modelBuilder.Entity<LogFile>(entity =>
            {
                entity.ToTable("LogFile");
                entity.HasKey(e => e.LogId);
                entity.Property(e => e.LogId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.LogDetails).IsRequired();
                entity.Property(e => e.LogTimeStamp).IsRequired();

                // Relationship : A LogFile belongs to one User
                entity.HasOne(e => e.Users)
                    .WithMany(u => u.LogFiles)
                    .HasForeignKey(e => e.UserId);
            });

            // Configuration for the 'Orders' entity.
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Property(e => e.OrderStatus).IsRequired();
                entity.Property(e => e.DateInitiated).IsRequired();
                entity.Property(e => e.DateCompleted).IsRequired(false);

                // Relationship: An order is associated with one customer
                entity.HasOne(e => e.Customer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.CustomerId);

                // Relationship: An Order is associated with one SourceCity
                entity.HasOne(e => e.SourceCity)
                    .WithMany(c => c.SourceOrders)
                    .HasForeignKey(e => e.SourceCityId);

                // Relationship: An Order is associated with one DestinationCity
                entity.HasOne(e => e.DestinationCity)
                    .WithMany(c => c.DestinationOrders)
                    .HasForeignKey(e => e.DestinationCityId);

                // Relationship: An order can have one invoice
                entity.HasOne(e => e.Invoices)
                    .WithOne(e => e.Order)
                    .HasForeignKey<Invoice>(i => i.OrderId)
                    .IsRequired();
            });


            modelBuilder.Entity<Rate>(entity =>
            {
                entity.ToTable("Rate");
                entity.HasKey(e => e.RateId);
                entity.Property(e => e.RateId).ValueGeneratedOnAdd();
                entity.Property(e => e.RateType).IsRequired();
                entity.Property(e => e.Amount).IsRequired();

                // Relationship : A Rate can be associated with multiple Invoices
                entity.HasMany(e => e.Invoices).WithOne(i => i.Rates).HasForeignKey(e => e.RateId);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");
                entity.HasKey(e => e.RouteId);
                entity.Property(e => e.RouteId).ValueGeneratedOnAdd();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.Duration).IsRequired();


                // Relationship with City: Each Route has one source city
                entity.HasOne(e => e.SourceCity)
                    .WithMany(c => c.SourceRoutes)
                    .HasForeignKey(e => e.SourceCityId);

                // Relationship with City: Each Route has one destination city
                entity.HasOne(e => e.DestinationCity)
                    .WithMany(c => c.DestinationRoutes)
                    .HasForeignKey(e => e.DestinationCityId);
            });


            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");
                entity.HasKey(e => e.TripId);
                entity.Property(e => e.TripId).ValueGeneratedOnAdd();
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.CarrierId).IsRequired();
                entity.Property(e => e.TripStatus).IsRequired();

               

               // Relationship: Each trip has one carrier
               entity.HasOne(e => e.Carrier).WithMany(c => c.Trips).HasForeignKey(e => e.CarrierId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.UserType).IsRequired();

                // Relationship: Each user can have multiple log files
                entity.HasMany(e => e.LogFiles).WithOne(l => l.Users).HasForeignKey(l => l.UserId);
            });
        }
    }
}