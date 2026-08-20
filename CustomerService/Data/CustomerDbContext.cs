using Microsoft.EntityFrameworkCore;
using CustomerService.Models.Entities;

namespace CustomerService.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<ComCustomer> ComCustomers => Set<ComCustomer>();
}