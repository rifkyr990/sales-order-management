namespace SalesOrderService.Models.DTOs;

public record OrderListDto(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    int CustomerId,
    string CustomerName,
    string? Address,
    decimal GrandTotal,
    int TotalCount
);

public record OrderDetailDto(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    int CustomerId,
    string CustomerName,
    string? Address,
    decimal GrandTotal,
    List<OrderItemDetailDto> Items
);

public record OrderItemDetailDto(
    int SalesSoLitemId,
    string ItemName,
    int Quantity,
    decimal Price,
    decimal Total
);

public record CreateOrderDto(
    string SoNo,
    DateTime OrderDate,
    int CustomerId,
    string? Address,
    List<CreateOrderItemDto> Items
);

public record CreateOrderItemDto(
    string ItemName,
    int Quantity,
    decimal Price
);

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}