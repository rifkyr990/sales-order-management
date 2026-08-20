using SalesOrderService.Models.DTOs;

namespace SalesOrderService.Services.Interfaces;

public interface ISalesOrderService
{
    Task<IEnumerable<OrderListDto>> GetOrdersAsync(string? keyword, DateTime? orderDate);
    Task<OrderDetailDto?> GetOrderByIdAsync(int id);
    Task<(bool Success, int SalesSoId, string Message, List<string>? Errors)> CreateOrderAsync(CreateOrderDto dto);