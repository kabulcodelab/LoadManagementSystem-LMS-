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
        var deliveredLoads = await query.CountAsync(x => x.Status == LoadStatus.Delivered);
        var totalRevenue = await query.Where(x => x.Status == LoadStatus.Delivered).SumAsync(x => x.Amount ?? 0);

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
            DeliveredLoads = deliveredLoads,
            TotalRevenue = totalRevenue,
            Drivers = drivers,
            Vehicles = vehicles,
            Customers = customers,
            RecentLoads = recentLoads
        };
    }
}