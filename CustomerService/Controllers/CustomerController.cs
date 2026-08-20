using Microsoft.AspNetCore.Mvc;
using CustomerService.Services;
using CustomerService.Services.Interfaces;

namespace CustomerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _customerService.GetAllCustomersAsync();
        return Ok(result);
    }
}