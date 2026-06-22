using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardData> GetDashboardData(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.Loads.AsQueryable();
        if (from.HasValue) query = query.Where(x => x.CreatedDate >= from.Value);
        if (to.HasValue) query = query.Where(x => x.CreatedDate <= to.Value);

        var totalLoads = await query.CountAsync();
        var pendingLoads = await query.CountAsync(x => x.Status == LoadStatus.Pending);
        var inTransitLoads = await query.CountAsync(x => x.Status == LoadStatus.InTransit);
        var deliveredLoads = await query.CountAsync(x => x.Status == LoadStatus.Delivered);
        var cancelledLoads = await query.CountAsync(x => x.Status == LoadStatus.Cancelled);

        // ✅ فقط مبلغ بارهایی که پرداخت شده‌اند (صرف‌نظر از وضعیت تحویل)
        var totalRevenue = await query
            .Where(x => x.IsPaid == true)
            .SumAsync(x => x.Amount ?? 0);

        var drivers = await _context.Drivers.CountAsync();
        var vehicles = await _context.Vehicles.CountAsync();
        var customers = await _context.Customers.CountAsync();

        var recentLoads = await query
            .Include(x => x.Customer)
            .OrderByDescending(x => x.CreatedDate)
            .Take(5)
            .Select(x => new RecentLoadDto
            {
                Id = x.Id,
                LoadNumber = x.LoadNumber,
                CustomerName = x.Customer!.Name,
                Status = x.Status,
                Amount = x.Amount,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync();

        return new DashboardData
        {
            TotalLoads = totalLoads,
            PendingLoads = pendingLoads,
            InTransitLoads = inTransitLoads,
            DeliveredLoads = deliveredLoads,
            CancelledLoads = cancelledLoads,
            TotalRevenue = totalRevenue,
            Drivers = drivers,
            Vehicles = vehicles,
            Customers = customers,
            RecentLoads = recentLoads
        };
    }

    public async Task<LoadStatusDistribution> GetLoadStatusDistribution(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.Loads.AsQueryable();
        if (from.HasValue) query = query.Where(x => x.CreatedDate >= from.Value);
        if (to.HasValue) query = query.Where(x => x.CreatedDate <= to.Value);

        return new LoadStatusDistribution
        {
            Pending = await query.CountAsync(x => x.Status == LoadStatus.Pending),
            InTransit = await query.CountAsync(x => x.Status == LoadStatus.InTransit),
            Delivered = await query.CountAsync(x => x.Status == LoadStatus.Delivered),
            Cancelled = await query.CountAsync(x => x.Status == LoadStatus.Cancelled)
        };
    }

    public async Task<Dictionary<DateTime, decimal>> GetDailyRevenue(int days = 7)
    {
        var startDate = DateTime.Today.AddDays(-days + 1);

        // ✅ فقط مبلغ بارهایی که پرداخت شده‌اند (صرف‌نظر از وضعیت تحویل)
        var dailyRevenue = await _context.Loads
            .Where(x => x.IsPaid == true && x.CreatedDate >= startDate)
            .GroupBy(x => x.CreatedDate.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount ?? 0) })
            .ToDictionaryAsync(x => x.Date, x => x.Total);

        var result = new Dictionary<DateTime, decimal>();
        for (var date = startDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            result[date] = dailyRevenue.ContainsKey(date) ? dailyRevenue[date] : 0;
        }

        return result;
    }
}