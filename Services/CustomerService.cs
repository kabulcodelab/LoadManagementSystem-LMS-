using LoadManagementSystem_LMS_.Data;
using LoadManagementSystem_LMS_.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadManagementSystem_LMS_.Services;

public class CustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAll()
    {
        return await _context.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetById(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task Add(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Load>> GetCustomerLoads(int customerId)
    {
        return await _context.Loads
            .Include(x => x.Driver)
            .Include(x => x.Vehicle)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }
    public async Task<List<Customer>> GetFiltered(string? search)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.CustomerCode.Contains(search) ||
                c.Name.Contains(search) ||
                c.PhoneNumber.Contains(search) ||
                c.Email.Contains(search)
            );
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}