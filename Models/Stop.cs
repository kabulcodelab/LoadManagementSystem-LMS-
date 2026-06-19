using LoadManagementSystem_LMS_.Models;

public class Stop
{
    public int Id { get; set; }
    public int LoadId { get; set; }
    public Load? Load { get; set; }

    public StopType Type { get; set; } = StopType.Pickup;
    public string? Company { get; set; }
    public DateTime? InitialDateTime { get; set; }
    public DateTime? FinalDateTime { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PickupNumber { get; set; }
    public string? Remarks { get; set; }
    public decimal? LoadedMiles { get; set; }
    public decimal? EmptyMiles { get; set; }
    public int Sequence { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum StopType { Pickup, Delivery }