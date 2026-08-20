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
    public async Task<IActionResult> GetOrders([FromQuery] string? search)
    {
        var orders = await _orderService.GetOrdersAsync(search);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { Message = "Order harus memiliki minimal 1 item." });

        var success = await _orderService.CreateOrderAsync(dto);
        if (!success) return BadRequest(new { Message = "Gagal menyimpan order." });

        return Ok(new { Message = "Sales Order berhasil disimpan." });
    }
}