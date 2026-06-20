using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadManagementSystem_LMS_.Models
{
    public class Load
    {
        public int Id { get; set; }

        // ========== Basic Information ==========
        
        public string LoadNumber { get; set; } = string.Empty;

        public string? TrackingNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [Required]
        public int DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }

        // ========== Load Type & Status ==========
        public LoadType Type { get; set; } = LoadType.PO;
        public LoadStatus Status { get; set; } = LoadStatus.Pending;

        // ========== Dates ==========
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? UpdatedAt { get; set; } // ✅ اضافه شد

        // ========== Financial ==========
        public decimal? Amount { get; set; }
        public bool IsPaid { get; set; } = false;

        // ========== Additional Info ==========
        [StringLength(500)]
        public string? Remarks { get; set; }

        // ========== Navigation Properties ==========
        public List<Stop> Stops { get; set; } = new List<Stop>(); // ✅ تغییر به List
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }

    public enum LoadType
    {
        PO, VAN, Flatbed
    }

    public enum LoadStatus
    {
        Pending, Assigned, PickedUp, InTransit, Delivered, Cancelled
    }
}