using SalesOrderService.Models.DTOs;

namespace SalesOrderService.Services.Interfaces;

public interface ISalesOrderService
{
    Task<PagedResult<OrderListDto>> GetOrdersAsync(string? keyword, DateTime? orderDate, int pageNumber, int pageSize);
    Task<OrderDetailDto?> GetOrderByIdAsync(int id);
    Task<(bool Success, int SalesSoId, string Message, List<string>? Errors)> CreateOrderAsync(CreateOrderDto dto);
    Task<(bool Success, string Message)> UpdateOrderAsync(int id, CreateOrderDto dto);
    Task<bool> DeleteOrderAsync(int id);
    Task<byte[]> ExportOrdersToExcelAsync(string? keyword, DateTime? orderDate);
}