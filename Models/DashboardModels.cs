using LoadManagementSystem_LMS_.Models;

namespace LoadManagementSystem_LMS_.Models
{
    public class DashboardData
    {
        public int TotalLoads { get; set; }
        public int PendingLoads { get; set; }
        public int InTransitLoads { get; set; }
        public int DeliveredLoads { get; set; }
        public int CancelledLoads { get; set; }
        public decimal TotalRevenue { get; set; }
        public int Drivers { get; set; }
        public int Vehicles { get; set; }
        public int Customers { get; set; }
        public List<RecentLoadDto> RecentLoads { get; set; } = new();
    }

    public class RecentLoadDto
    {
        public int Id { get; set; }
        public string LoadNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public LoadStatus Status { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class LoadStatusDistribution
    {
        public int Pending { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }
}