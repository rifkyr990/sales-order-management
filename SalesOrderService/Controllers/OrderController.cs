using Microsoft.AspNetCore.Mvc;
using SalesOrderService.Models.DTOs;
using SalesOrderService.Services.Interfaces;

namespace SalesOrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISalesOrderService _orderService;

    public OrdersController(ISalesOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] string? keyword, [FromQuery] DateTime? orderDate)
    {
        var orders = await _orderService.GetOrdersAsync(keyword, orderDate);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound(new { success = false, message = "Order tidak ditemukan" });

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var result = await _orderService.CreateOrderAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        return StatusCode(201, new
        {
            success = true,
            salesSoId = result.SalesSoId,
            message = result.Message
        });
    }
}