using SalesOrderService.Models.DTOs;

namespace SalesOrderService.Services.Interfaces;

public interface ISalesOrderService
{
    Task<IEnumerable<OrderListDto>> GetOrdersAsync(string? search);
    Task<bool> CreateOrderAsync(CreateOrderDto dto);
}