using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class DriverService
{
    private readonly ApplicationDbContext _context;

    public DriverService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔥 NEW: GET ALL (basic)
    public async Task<List<Driver>> GetAll()
    {
        return await _context.Drivers
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    // 🔥 NEW: SEARCH (professional)
    public async Task<List<Driver>> GetFiltered(string? search)
    {
        var query = _context.Drivers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.PhoneNumber.Contains(search) ||
                x.LicenseNumber.Contains(search)
            );
        }

        return await query
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Driver?> GetById(int id)
    {
        return await _context.Drivers.FindAsync(id);
    }

    public async Task Add(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Driver driver)
    {
        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver != null)
        {
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Load>> GetDriverLoads(int driverId)
    {
        return await _context.Loads
            .Include(x => x.Customer)
            .Include(x => x.Vehicle)
            .Where(x => x.DriverId == driverId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }
}