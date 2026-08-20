using CustomerService.Models.DTOs;

namespace CustomerService.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
}