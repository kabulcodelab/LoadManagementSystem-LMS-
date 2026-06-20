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
    // GET PAGED (with search, date, status, type, and isPaid)
    // ================================================
    public async Task<PagedResult<Load>> GetPaged(
        int page,
        int pageSize = 10,
        string? search = null,
        DateTime? from = null,
        DateTime? to = null,
        LoadStatus? status = null,
        LoadType? type = null,
        bool? isPaid = null)
    {
        var query = _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .AsQueryable();

        // 🔍 Search
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

        // 📅 Date filter
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

        // 💰 Payment filter
        if (isPaid.HasValue)
            query = query.Where(x => x.IsPaid == isPaid.Value);

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
            .Include(x => x.Stops.OrderBy(s => s.Sequence))
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // ================================================
    // ADD (with auto-generate LoadNumber & sequence)
    // ================================================
    public async Task Add(Load load)
    {
        
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
        else
        {
            var exists = await _context.Loads
                .AnyAsync(l => l.LoadNumber == load.LoadNumber);

            if (exists)
                throw new InvalidOperationException($"Load number '{load.LoadNumber}' already exists. Please use a unique number.");
        }

        // ================================================
        // 2.  Stops
        // ================================================
        int seq = 1;
        foreach (var stop in load.Stops)
        {
            stop.Sequence = seq++;
            stop.CreatedAt = DateTime.UtcNow;
            stop.LoadId = 0;
        }

        // ================================================
        // 3. date
        // ================================================
        load.CreatedDate = DateTime.Now;

        // ================================================
        // 4. save to the database
        // ================================================
        _context.Loads.Add(load);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // UPDATE (full update with sequence update)
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

        // Set UpdatedAt for the Load itself
        load.UpdatedAt = DateTime.UtcNow;

        _context.Loads.Update(load);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // UPDATE STATUS ONLY (for inline editing)
    // ================================================
    public async Task UpdateStatus(int loadId, LoadStatus newStatus)
    {
        var load = await _context.Loads.FindAsync(loadId);
        if (load == null)
            throw new Exception("Load not found.");

        load.Status = newStatus;
        load.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // ================================================
    // UPDATE PAYMENT STATUS ONLY (for inline editing)
    // ================================================
    public async Task UpdatePaidStatus(int loadId, bool isPaid)
    {
        var load = await _context.Loads.FindAsync(loadId);
        if (load == null)
            throw new Exception("Load not found.");

        load.IsPaid = isPaid;
        load.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // ================================================
    // DELETE (with related entities)
    // ================================================
    public async Task Delete(int id)
    {
        var load = await _context.Loads
            .Include(x => x.Stops)
            .Include(x => x.Documents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (load != null)
        {
            _context.Stops.RemoveRange(load.Stops);
            _context.Documents.RemoveRange(load.Documents);
            _context.Loads.Remove(load);
            await _context.SaveChangesAsync();
        }
    }

    // ================================================
    // FILTER (legacy – kept for compatibility)
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

    // ================================================
    // DETACH LOAD (to prevent accidental saves)
    // ================================================
    public void DetachLoad(Load load)
    {
        if (load != null)
            _context.Entry(load).State = EntityState.Detached;
    }
}