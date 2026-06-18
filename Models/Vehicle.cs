using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadManagementSystem_LMS_.Models;

public class Vehicle
{
    public int Id { get; set; }

    // ========== ID and Code ==========
    
    public string VehicleCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Plate Number is required")]
    public string PlateNumber { get; set; } = string.Empty;

   
    public string VIN { get; set; } = string.Empty;  // Chassis number

    public string? EngineNumber { get; set; }        // Engine number (optional)

    // ========== Technical Specifications ==========
    [Required(ErrorMessage = "Model is required")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle Type is required")]
    public string VehicleType { get; set; } = string.Empty;  // e.g., "Truck", "Trailer", "Van"

    public string? Color { get; set; }  // Color (optional)

    [Required(ErrorMessage = "Capacity is required")]
    [Range(0.1, double.MaxValue, ErrorMessage = "Capacity must be greater than zero")]
    public decimal Capacity { get; set; }  // in kilograms

    public string? CapacityUnit { get; set; } = "kg";  // Capacity unit (default: kg)

    
    public int ManufactureYear { get; set; }

    // ========== Operational Information ==========
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    public int? CurrentOdometer { get; set; }          // Current odometer reading (optional)
    public DateTime? LastServiceDate { get; set; }     // Last service date
    public int? LastServiceOdometer { get; set; }      // Odometer reading at last service
    public decimal? FuelConsumption { get; set; }      // Liters per 100 km

    // ========== Financial Information ==========
    public decimal? CostPerKm { get; set; }            // Cost per kilometer
    public DateTime? PurchaseDate { get; set; }        // Purchase date
    public decimal? PurchasePrice { get; set; }        // Purchase price
    public DateTime? WarrantyExpiryDate { get; set; }  // Warranty expiry date

    // ========== Documents and Validity ==========
    public DateTime? InspectionExpiryDate { get; set; } // Technical inspection expiry
    public DateTime? InsuranceExpiryDate { get; set; }  // Insurance expiry

    // ========== General Status ==========
    public bool IsActive { get; set; } = true;

    // ========== Driver Relationship ==========
    public int? CurrentDriverId { get; set; }           // Current driver (optional)
    [ForeignKey(nameof(CurrentDriverId))]
    public Driver? CurrentDriver { get; set; }

    public ICollection<Driver>? DriversHistory { get; set; } = new List<Driver>(); // History of assigned drivers

    // ========== System Fields ==========
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ========== Load Relationship ==========
    public ICollection<Load> Loads { get; set; } = new List<Load>();
}

public enum VehicleStatus
{
    Available,        // Ready to work
    OnTrip,           // Currently on a trip
    UnderMaintenance, // At the workshop
    OutOfService,     // Out of service (broken or retired)
    Reserved          // Reserved for a specific load
}