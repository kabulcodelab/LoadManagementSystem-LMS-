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

    // ================================================
    // GET ALL (for dropdowns / simple lists)
    // ================================================
    public async Task<List<Load>> GetAll()
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    // ================================================
    // GET PAGED (with search, date filter, status filter)
    // ================================================
    public async Task<PagedResult<Load>> GetPaged(
        int page,
        int pageSize = 10,
        string? search = null,
        DateTime? from = null,
        DateTime? to = null,
        LoadStatus? status = null,
        LoadType? type = null)
    {
        var query = _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .AsQueryable();

        // 🔍 Search (LoadNumber, Tracking, Customer, Driver, Vehicle Plate)
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.LoadNumber.Contains(search) ||
                (x.TrackingNumber != null && x.TrackingNumber.Contains(search)) ||
                (x.Customer != null && x.Customer.Name.Contains(search)) ||
                (x.Driver != null && x.Driver.FullName.Contains(search)) ||
                (x.Vehicle != null && x.Vehicle.PlateNumber.Contains(search))
            );
        }

        // 📅 Date filter (CreatedDate range)
        if (from.HasValue)
            query = query.Where(x => x.CreatedDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedDate <= to.Value);

        // 🚦 Status filter
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        // 🏷️ Type filter
        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Load>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
    }

    // ================================================
    // GET BY ID (with Stops and Documents)
    // ================================================
    public async Task<Load?> GetById(int id)
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .Include(x => x.Stops.OrderBy(s => s.Sequence))  // Stops ordered by sequence
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // ================================================
    // ADD (with auto-generate LoadNumber & sequence)
    // ================================================
    public async Task Add(Load load)
    {
        // Auto-generate LoadNumber if empty
        if (string.IsNullOrWhiteSpace(load.LoadNumber))
        {
            var lastLoad = await _context.Loads
                .OrderByDescending(l => l.Id)
                .Select(l => l.LoadNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastLoad))
            {
                var parts = lastLoad.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            load.LoadNumber = $"LON-{nextNumber:D3}";
        }

        // Set sequence for Stops
        int seq = 1;
        foreach (var stop in load.Stops)
        {
            stop.Sequence = seq++;
            stop.CreatedAt = DateTime.UtcNow;
            stop.LoadId = 0; // Will be set after load is added
        }

        load.CreatedDate = DateTime.Now;

        _context.Loads.Add(load);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // UPDATE (with sequence update)
    // ================================================
    public async Task Update(Load load)
    {
        // Update sequence for Stops
        int seq = 1;
        foreach (var stop in load.Stops)
        {
            stop.Sequence = seq++;
            stop.UpdatedAt = DateTime.UtcNow;
        }

        _context.Loads.Update(load);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // DELETE
    // ================================================
    public async Task Delete(int id)
    {
        var load = await _context.Loads
            .Include(x => x.Stops)
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (load != null)
        {
            // Remove related Stops and Documents (cascade delete should handle if configured)
            _context.Stops.RemoveRange(load.Stops);
            _context.Documents.RemoveRange(load.Documents);
            _context.Loads.Remove(load);
            await _context.SaveChangesAsync();
        }
    }

    // ================================================
    // FILTER (legacy - keep for compatibility)
    // ================================================
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

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.LoadNumber.Contains(search) ||
                (x.TrackingNumber != null && x.TrackingNumber.Contains(search)) ||
                (x.Customer != null && x.Customer.Name.Contains(search)) ||
                (x.Driver != null && x.Driver.FullName.Contains(search)) ||
                (x.Vehicle != null && x.Vehicle.PlateNumber.Contains(search))
            );
        }

        if (from.HasValue)
            query = query.Where(x => x.CreatedDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedDate <= to.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.OrderByDescending(x => x.CreatedDate).ToListAsync();
    }
}