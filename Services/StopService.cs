using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class StopService
{
    private readonly ApplicationDbContext _context;

    public StopService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================================================
    // GET STOPS BY LOAD ID (ordered by Sequence)
    // ================================================
    public async Task<List<Stop>> GetByLoadId(int loadId)
    {
        return await _context.Stops
            .Where(s => s.LoadId == loadId)
            .OrderBy(s => s.Sequence)
            .ToListAsync();
    }

    // ================================================
    // GET STOP BY ID
    // ================================================
    public async Task<Stop?> GetById(int id)
    {
        return await _context.Stops
            .Include(s => s.Load)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    // ================================================
    // ADD A NEW STOP (auto-sequence)
    // ================================================
    public async Task Add(Stop stop)
    {
        // Get the next sequence number for this load
        var maxSequence = await _context.Stops
            .Where(s => s.LoadId == stop.LoadId)
            .MaxAsync(s => (int?)s.Sequence) ?? 0;

        stop.Sequence = maxSequence + 1;
        stop.CreatedAt = DateTime.UtcNow;

        _context.Stops.Add(stop);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // ADD MULTIPLE STOPS (bulk add)
    // ================================================
    public async Task AddRange(List<Stop> stops)
    {
        if (stops == null || !stops.Any())
            return;

        var loadId = stops.First().LoadId;
        var maxSequence = await _context.Stops
            .Where(s => s.LoadId == loadId)
            .MaxAsync(s => (int?)s.Sequence) ?? 0;

        int seq = maxSequence;
        foreach (var stop in stops)
        {
            seq++;
            stop.Sequence = seq;
            stop.CreatedAt = DateTime.UtcNow;
        }

        _context.Stops.AddRange(stops);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // UPDATE A STOP
    // ================================================
    public async Task Update(Stop stop)
    {
        stop.UpdatedAt = DateTime.UtcNow;
        _context.Stops.Update(stop);
        await _context.SaveChangesAsync();
    }

    // ================================================
    // DELETE A STOP
    // ================================================
    public async Task Delete(int id)
    {
        var stop = await _context.Stops.FindAsync(id);
        if (stop != null)
        {
            _context.Stops.Remove(stop);
            await _context.SaveChangesAsync();
        }
    }

    // ================================================
    // DELETE ALL STOPS FOR A LOAD (cascade)
    // ================================================
    public async Task DeleteByLoadId(int loadId)
    {
        var stops = await _context.Stops
            .Where(s => s.LoadId == loadId)
            .ToListAsync();

        if (stops.Any())
        {
            _context.Stops.RemoveRange(stops);
            await _context.SaveChangesAsync();
        }
    }

    // ================================================
    // REORDER STOPS (update Sequence after reorder)
    // ================================================
    public async Task Reorder(int loadId, List<int> orderedStopIds)
    {
        var stops = await _context.Stops
            .Where(s => s.LoadId == loadId)
            .ToListAsync();

        var stopDict = stops.ToDictionary(s => s.Id);

        int seq = 1;
        foreach (var id in orderedStopIds)
        {
            if (stopDict.TryGetValue(id, out var stop))
            {
                stop.Sequence = seq++;
                stop.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    // ================================================
    // GET STOPS BY TYPE (Pickup/Delivery)
    // ================================================
    public async Task<List<Stop>> GetByType(int loadId, StopType type)
    {
        return await _context.Stops
            .Where(s => s.LoadId == loadId && s.Type == type)
            .OrderBy(s => s.Sequence)
            .ToListAsync();
    }

    // ================================================
    // GET ALL STOPS WITH LOAD INCLUDED (for paged list)
    // ================================================
    public async Task<List<Stop>> GetStopsWithLoad()
    {
        return await _context.Stops
            .Include(s => s.Load)
            .OrderBy(s => s.LoadId)
            .ThenBy(s => s.Sequence)
            .ToListAsync();
    }
}