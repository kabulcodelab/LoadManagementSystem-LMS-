namespace LoadManagementSystem_LMS_.Models;

public class Driver
{
    public int Id { get; set; }

    public string DriverCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public ICollection<Load> Loads { get; set; } = new List<Load>();
}