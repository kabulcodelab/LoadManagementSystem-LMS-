using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class VehicleService
{
    private readonly ApplicationDbContext _context;

    public VehicleService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== GET ALL (without pagination, for dropdowns) =====
    public async Task<List<Vehicle>> GetAll()
    {
        return await _context.Vehicles
            .OrderBy(v => v.VehicleCode)
            .ToListAsync();
    }

    // ===== GET BY ID (with includes) =====
    public async Task<Vehicle?> GetById(int id)
    {
        return await _context.Vehicles
            .Include(v => v.CurrentDriver)
            .Include(v => v.Loads)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    // ===== GET PAGED (with search) =====
    public async Task<PagedResult<Vehicle>> GetPaged(int page, int pageSize, string? search = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.VehicleCode.Contains(search) ||
                v.PlateNumber.Contains(search) ||
                v.VIN.Contains(search) ||
                v.Model.Contains(search) ||
                v.VehicleType.Contains(search) ||
                v.EngineNumber!.Contains(search)
            );
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(v => v.VehicleCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.CurrentDriver)  // for showing driver name
            .ToListAsync();

        return new PagedResult<Vehicle>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
    }

    // ===== CRUD =====
    public async Task Add(Vehicle vehicle)
    {
        // Auto-generate VehicleCode if empty
        if (string.IsNullOrWhiteSpace(vehicle.VehicleCode))
            vehicle.VehicleCode = $"VEH-{DateTime.Now:yyyyMMddHHmmss}";

        vehicle.CreatedAt = DateTime.UtcNow;
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Vehicle vehicle)
    {
        vehicle.UpdatedAt = DateTime.UtcNow;
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }

    // ===== LOADS HISTORY =====
    public async Task<List<Load>> GetVehicleLoads(int vehicleId)
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    // ===== LEGACY FILTER (for backward compatibility, can be removed) =====
    public async Task<List<Vehicle>> GetFiltered(string? search)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.VehicleCode.Contains(search) ||
                v.PlateNumber.Contains(search) ||
                v.Model.Contains(search) ||
                v.VehicleType.Contains(search)
            );
        }

        return await query
            .OrderBy(v => v.VehicleCode)
            .ToListAsync();
    }
}