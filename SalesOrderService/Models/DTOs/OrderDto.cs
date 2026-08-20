namespace SalesOrderService.Models.DTOs;

public record OrderListDto(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    int CustomerId,
    string CustomerName,
    string? Address,
    decimal GrandTotal
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