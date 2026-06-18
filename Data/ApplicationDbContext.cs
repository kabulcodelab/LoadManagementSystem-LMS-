using LoadManagementSystem_LMS_.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Data
{
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================================================
            // 1. Configure the relationship between Driver and Vehicle
            // ================================================

            // Primary relationship: Each driver can be assigned to one vehicle (optional)
            // and each vehicle can have multiple drivers in its history
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.AssignedVehicle)
                .WithMany(v => v.DriversHistory)  // Use DriversHistory as the inverse navigation
                .HasForeignKey(d => d.AssignedVehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // One-to-one relationship for the vehicle's current driver (via Vehicle.CurrentDriverId)
            // This relationship is independent from the above and has its own foreign key
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.CurrentDriver)
                .WithOne() // No corresponding navigation property in Driver (since Driver already has AssignedVehicle)
                .HasForeignKey<Vehicle>(v => v.CurrentDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // ================================================
            // 2. Unique index on VIN (only for non-empty values)
            // ================================================

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VIN)
                .IsUnique()
                .HasFilter("[VIN] IS NOT NULL AND [VIN] != ''");
            // This filter ensures that records with empty VIN (null or "") are not included
            // in the uniqueness check, preventing duplicate key errors

            // ================================================
            // 3. (Optional) Additional configurations
            // ================================================

            // Set a default value for Status
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Status)
                .HasDefaultValue(VehicleStatus.Available);

            // Set precision and scale for decimal fields
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Capacity)
                .HasPrecision(18, 2);
        }
    }
}