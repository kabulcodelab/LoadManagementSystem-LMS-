using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class LoadService
{
    private readonly ApplicationDbContext _context;

    public LoadService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ GET ALL (default)
    public async Task<List<Load>> GetAll()
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    // 🔥 FILTER + SEARCH (NEW)
    public async Task<List<Load>> GetFiltered(
        string? search,
        DateTime? from,
        DateTime? to,
        LoadStatus? status)
    {
        var query = _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .AsQueryable();

        // 🔍 SEARCH (LoadNo, Tracking, Customer, Driver, Plate)
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.LoadNumber.Contains(search) ||
                x.TrackingNumber.Contains(search) ||
                x.Customer!.Name.Contains(search) ||
                x.Driver!.FullName.Contains(search) ||
                x.Vehicle!.PlateNumber.Contains(search)
            );
        }

        // 📅 DATE FILTER
        if (from.HasValue)
        {
            query = query.Where(x => x.CreatedDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.CreatedDate <= to.Value);
        }

        // 🚦 STATUS FILTER
        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
    }

    public async Task<Load?> GetById(int id)
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Load load)
    {
        _context.Loads.Add(load);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Load load)
    {
        _context.Loads.Update(load);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var load = await _context.Loads.FindAsync(id);

        if (load != null)
        {
            _context.Loads.Remove(load);
            await _context.SaveChangesAsync();
        }
    }
}