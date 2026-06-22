using System.ComponentModel.DataAnnotations;

namespace LoadManagementSystem_LMS_.Models;

public class Driver
{
    public int Id { get; set; }

    // Optional - auto generated, but not required for user input
    public string DriverCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "National Code is required")]
    public string NationalCode { get; set; } = string.Empty;

    [RegularExpression(@"^\+1[0-9]{10}$",
    ErrorMessage = "Phone Number must be in format +1XXXXXXXXXX")]
    public string PhoneNumber { get; set; } = string.Empty;

    // Optional fields
    public string? AlternatePhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    [Required(ErrorMessage = "License Number is required")]
    public string LicenseNumber { get; set; } = string.Empty;

    public string? LicenseClass { get; set; }

    public DateTime? LicenseExpiryDate { get; set; }

    public DateTime? MedicalExamExpiryDate { get; set; }

    public string? BankAccountNumber { get; set; }

    public decimal? SalaryBaseRate { get; set; }

    public bool IsActive { get; set; } = true;
    public DriverStatus Status { get; set; } = DriverStatus.Available;
    public DateTime? LastStatusChangeDate { get; set; }
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }

    public int? AssignedVehicleId { get; set; }
    public Vehicle? AssignedVehicle { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Load> Loads { get; set; } = new List<Load>();
}

public enum DriverStatus
{
    Available,
    OnTrip,
    Resting,
    Sick,
    OffDuty,
    Unreachable
}