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

    public async Task<List<Vehicle>> GetAll()
    {
        return await _context.Vehicles
            .OrderBy(v => v.VehicleCode)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetById(int id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task Add(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Vehicle vehicle)
    {
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
    public async Task<List<Load>> GetVehicleLoads(int vehicleId)
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Driver)
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }
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