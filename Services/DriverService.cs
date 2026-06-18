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

    public async Task<List<Driver>> GetAll()
    {
        return await _context.Drivers
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<Driver>> GetFiltered(string? search)
    {
        var query = _context.Drivers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.PhoneNumber.Contains(search) ||
                x.LicenseNumber.Contains(search) ||
                x.NationalCode.Contains(search) ||
                x.DriverCode.Contains(search)
            );
        }

        return await query
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<Driver?> GetById(int id)
    {
        return await _context.Drivers
            .Include(d => d.AssignedVehicle)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task Add(Driver driver)
    {
        // Auto-generate DriverCode if empty
        if (string.IsNullOrWhiteSpace(driver.DriverCode))
            driver.DriverCode = $"DRV-{DateTime.Now:yyyyMMddHHmmss}";

        // Set creation timestamp
        driver.CreatedAt = DateTime.UtcNow;

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Driver driver)
    {
        driver.UpdatedAt = DateTime.UtcNow;
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

    public async Task<PagedResult<Driver>> GetPaged(int page, int pageSize, string? search = null)
    {
        var query = _context.Drivers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.PhoneNumber.Contains(search) ||
                x.LicenseNumber.Contains(search) ||
                x.NationalCode.Contains(search) ||
                x.DriverCode.Contains(search)
            );
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Driver>
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
    }
}