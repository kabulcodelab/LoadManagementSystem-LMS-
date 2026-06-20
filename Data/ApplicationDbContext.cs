using LoadManagementSystem_LMS_.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Data
{
    // ✅ تغییر از IdentityDbContext به IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Load> Loads { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Stop> Stops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================================================
            // Vehicle Plate Number Unique
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber)
                .IsUnique()
                .HasFilter("[PlateNumber] IS NOT NULL AND [PlateNumber] <> ''");

            // ================================================
            // Vehicle VIN Unique
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VIN)
                .IsUnique()
                .HasFilter("[VIN] IS NOT NULL AND [VIN] <> ''");

            // ================================================
            // Driver -> Assigned Vehicle
            // ================================================
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.AssignedVehicle)
                .WithMany(v => v.DriversHistory)
                .HasForeignKey(d => d.AssignedVehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // ================================================
            // Vehicle -> Current Driver
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.CurrentDriver)
                .WithOne()
                .HasForeignKey<Vehicle>(v => v.CurrentDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // ================================================
            // Default Values
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Status)
                .HasDefaultValue(VehicleStatus.Available);

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Capacity)
                .HasPrecision(18, 2);

            // ================================================
            // Stop - unique index on (LoadId, Sequence)
            // ================================================
            modelBuilder.Entity<Stop>()
                .HasIndex(s => new { s.LoadId, s.Sequence })
                .IsUnique()
                .HasFilter("[LoadId] IS NOT NULL AND [Sequence] IS NOT NULL");

            // ================================================
            // Stop - relationship to Load (Cascade Delete)
            // ================================================
            modelBuilder.Entity<Stop>()
                .HasOne(s => s.Load)
                .WithMany(l => l.Stops)
                .HasForeignKey(s => s.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Document - relationship to Load (Cascade Delete)
            // ================================================
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Load)
                .WithMany(l => l.Documents)
                .HasForeignKey(d => d.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Load - default values
            // ================================================
            modelBuilder.Entity<Load>()
                .Property(l => l.Type)
                .HasDefaultValue(LoadType.PO);

            modelBuilder.Entity<Load>()
                .Property(l => l.Status)
                .HasDefaultValue(LoadStatus.Pending);
        }
    }
}