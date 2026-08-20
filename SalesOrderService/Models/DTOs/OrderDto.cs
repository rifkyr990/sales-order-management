namespace SalesOrderService.Models.DTOs;

public record OrderListDto(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    string CustomerName,
    double TotalAmount
);

public record CreateOrderDto(
    string SoNo,
    DateTime OrderDate,
    int ComCustomerId,
    string? Address,
    List<CreateOrderItemDto> Items
);

public record CreateOrderItemDto(
    string ItemName,
    int Quantity,
    double Price
);