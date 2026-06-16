namespace LoadManagementSystem_LMS_.Models;

public class Load
{
    public int Id { get; set; }

    public string LoadNumber { get; set; } = string.Empty;

    public string TrackingNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public int DriverId { get; set; }

    public Driver? Driver { get; set; }

    public int VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    public string PickupLocation { get; set; } = string.Empty;

    public string DeliveryLocation { get; set; } = string.Empty;

    public decimal Weight { get; set; }

    public decimal Price { get; set; }

    public DateTime PickupDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public LoadStatus Status { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}