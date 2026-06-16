namespace LoadManagementSystem_LMS_.Models;

public class Vehicle
{
    public int Id { get; set; }

    public string VehicleCode { get; set; } = string.Empty;

    public string PlateNumber { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string VehicleType { get; set; } = string.Empty;

    public decimal Capacity { get; set; }

    public int ManufactureYear { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Load> Loads { get; set; } = new List<Load>();
}