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
            // 1. Unique index on PlateNumber (only for non-empty values)
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber)
                .IsUnique()
                .HasFilter("[PlateNumber] IS NOT NULL AND [PlateNumber] != ''");

            // ================================================
            // 2. Configure the relationship between Driver and Vehicle
            // ================================================
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.AssignedVehicle)
                .WithMany(v => v.DriversHistory)
                .HasForeignKey(d => d.AssignedVehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.CurrentDriver)
                .WithOne()
                .HasForeignKey<Vehicle>(v => v.CurrentDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // ================================================
            // 3. Unique index on VIN (with filter)
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VIN)
                .IsUnique()
                .HasFilter("[VIN] IS NOT NULL AND [VIN] != ''");

            // ================================================
            // 4. Additional configurations
            // ================================================
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Status)
                .HasDefaultValue(VehicleStatus.Available);

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Capacity)
                .HasPrecision(18, 2);
        }
    }
}