//using API.Authentication;
using BookingEngine.Data.Configuration;
using BookingEngine.Entities.Models;
using BookingEngine.Entities.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingEngine.Data
{
    // DbContext is a pre-build class that allows us to interact with our database
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        // Initiating a database connection with its entities
        public class OptionsBuild
        {
            public OptionsBuild()
            {
                settings = new AppConfiguration();
                optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
                optionsBuilder.UseSqlServer(settings.sqlConnectionString);
                databaseOptions = optionsBuilder.Options;
            }
            // DbContextOptionsBuilder is a pre-build API that allows us to configure the connection to our database
            public DbContextOptionsBuilder<DatabaseContext> optionsBuilder { get; set; }
            // DbContextOptions is a class that obtains our database information
            public DbContextOptions<DatabaseContext> databaseOptions { get; set; }
            // connection string
            private AppConfiguration settings { get; set; }
        }

        // making the OptionsBuild class accessible from other namespaces and classes
        public static OptionsBuild ops = new OptionsBuild();

        // constructor for the DbContext class
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        // DbSets

        // Users table was removed because the IdentityDbContext generates custom User Table and there were conflicts
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<AssociatedRecordItem> AssociatedRecords { get; set; }

        // generating authentication tables
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            builder.Seed();
        }
    }
}