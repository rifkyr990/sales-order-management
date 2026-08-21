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
    public async Task<IActionResult> GetOrders([FromQuery] string? keyword, [FromQuery] DateTime? orderDate,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
    {
        var result = await _orderService.GetOrdersAsync(keyword, orderDate, pageNumber, pageSize);
        return Ok(result);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrderDto dto)
    {
        var result = await _orderService.UpdateOrderAsync(id, dto);
        if (!result.Success)
            return NotFound(new { success = false, message = "Order tidak ditemukan" });

        return Ok(new { success = true, message = result.Message });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var success = await _orderService.DeleteOrderAsync(id);
        if (!success)
            return NotFound(new { success = false, message = "Order tidak ditemukan" });

        return Ok(new { success = true, message = "Order berhasil dihapus" });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportToExcel([FromQuery] string? keyword, [FromQuery] DateTime? orderDate)
    {
        var excelBytes = await _orderService.ExportOrdersToExcelAsync(keyword, orderDate);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesOrders.xlsx");
    }
}