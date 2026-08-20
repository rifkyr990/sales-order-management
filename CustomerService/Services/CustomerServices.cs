using Microsoft.EntityFrameworkCore;
using CustomerService.Data;
using CustomerService.Models.DTOs;
using CustomerService.Services.Interfaces;

namespace CustomerService.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerDbContext _context;

    public CustomerService(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        // Tempat logika bisnis (validasi, pemrosesan data, mapping Entity -> DTO)
        return await _context.ComCustomers
            .AsNoTracking()
            .Select(c => new CustomerDto(c.ComCustomerId, c.CustomerName))
            .ToListAsync();
    }
}