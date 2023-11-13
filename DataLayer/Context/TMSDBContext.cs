using DataLayer.Model;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DataLayer.Context
{
    // DbContext is the main class to interact with the database using Entity Framework Core.
    public class TmsDbContext : DbContext
    {
        // DbSet properties represent database tables.
        // Each DbSet corresponds to a model class and will be used for database operations.
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Rates> Rates { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<LogFile> LogFiles { get; set; }
        public DbSet<Invoice> InvoiceDetails { get; set; }

        // This method is used to configure the database connection.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection string contains information like server, port, database, user, and password.
            const string connectionString = "server=localhost; port=3306; database=tms; user=root; password=";

            // Configure the DbContext to use MySql with the provided connection string.
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        // This method is used to define the database model and relationships.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method to include any configurations from the base class.
            base.OnModelCreating(modelBuilder);

            // Configuration for the 'Customer' entity.
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer"); // Set the table name explicitly (optional, inferred from the class name)
                entity.HasKey(e => e.CustomerId); // Define the primary key
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd(); // Set auto-increment for the primary key
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired(); // Define constraints for 'Name'
                entity.Property(e => e.PhoneNumber); // 'PhoneNumber' property
                entity.Property(e => e.Email); // 'Email' property
            });

            // Configuration for the 'Carrier' entity.
            modelBuilder.Entity<Carrier>(entity =>
            {
                entity.ToTable("Carrier");
                entity.HasKey(e => e.CarrierId);
                entity.Property(e => e.CarrierId).ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyName).IsRequired(); // Assuming 'CompanyName' is required
                entity.Property(e => e.Capacity); // 'Capacity' property

                // Relationship: A Carrier can have multiple Trips
                entity.HasMany(e => e.Trips).WithOne(t => t.Carrier).HasForeignKey(t => t.CarrierId);
            });

            // Configuration for the 'Cities' entity.
            modelBuilder.Entity<Cities>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(e => e.CityId);
                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
                entity.Property(e => e.CityName).IsRequired(); // Assuming 'CityName' is required
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

                // Relationship : An Invoice belongs to one Order
                entity.HasOne(e => e.Order).WithMany(o => o.Invoices).HasForeignKey(e => e.OrderId);

                // Relationship : An Invoice has one Rate
                entity.HasOne(e => e.Rates).WithMany().HasForeignKey(e => e.RateId);
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
                entity.HasOne(e => e.Users).WithMany(u => u.LogFiles).HasForeignKey(e => e.UserId);
            });

            // Configuration for the 'Order' entity.
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
                entity.Property(e => e.ContractId).IsRequired();
                entity.Property(e => e.BuyerId).IsRequired();
                entity.Property(e => e.OrderStatus).IsRequired();
                entity.Property(e => e.DateInitiated).IsRequired();
                entity.Property(e => e.DateCompleted).IsRequired();
            });

            // Configuration for the 'Rates' entity.
            modelBuilder.Entity<Rates>(entity =>
            {
                entity.ToTable("Rates");
                entity.HasKey(e => e.RateId);
                entity.Property(e => e.RateId).ValueGeneratedOnAdd();
                entity.Property(e => e.RateType).IsRequired();
                entity.Property(e => e.Amount).IsRequired();

                // Relationship : A Rate can be associated with multiple Invoices
                entity.HasMany(e => e.Invoices).WithOne(i => i.Rates).HasForeignKey(e => e.RateId);
            });

            // Configuration for the 'Route' entity.
            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");
                entity.HasKey(e => e.RouteId);
                entity.Property(e => e.RouteId).ValueGeneratedOnAdd();
                entity.Property(e => e.SourceCityId).IsRequired();
                entity.Property(e => e.DestinationCityId).IsRequired();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.Duration).IsRequired();

                // Relationship : A Route has one source city
                entity.HasOne(e => e.SourceCity).WithMany().HasForeignKey(e => e.SourceCityId);

                // Relationship : A Route has one destination city
                entity.HasOne(e => e.DestinationCity).WithMany().HasForeignKey(e => e.DestinationCityId);
            });

            // Configuration for the 'Trip' entity.
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");
                entity.HasKey(e => e.TripId);
                entity.Property(e => e.TripId).ValueGeneratedOnAdd();
                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.CarrierId).IsRequired();
                entity.Property(e => e.TripStatus).IsRequired();

                // Relationship : A Trip belongs to one Order
                // object value = entity.HasOne(e => e.Order).WithMany(o => o.Trips).HasForeignKey(e => e.OrderId);

                // Relationship : A Trip belongs to one Carrier
                entity.HasOne(e => e.Carrier).WithMany(c => c.Trips).HasForeignKey(e => e.CarrierId);
            });

            // Configuration for the 'User' entity.
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.UserType).IsRequired();

                // Relationship : A User can have multiple Orders
                entity.HasMany(e => e.Orders).WithOne(o => o.Buyer).HasForeignKey(o => o.BuyerId);

                // Relationship : A User can have multiple LogFiles
                entity.HasMany(e => e.LogFiles).WithOne(l => l.Users).HasForeignKey(l => l.UserId);
            });
        }
    }
}
