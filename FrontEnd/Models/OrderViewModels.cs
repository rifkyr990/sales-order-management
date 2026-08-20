using System.Text.Json.Serialization;

namespace FrontEnd.Models;

public record OrderListDto(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    string CustomerName,
    string? Address,
    double GrandTotal
);

public class CreateOrderViewModel
{
    public string SoNo { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public string? Address { get; set; }
    public List<CreateOrderItemViewModel> Items { get; set; } = new();
}

public class CreateOrderItemViewModel
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CustomerOptionDto
{
    [JsonPropertyName("comCustomerId")]
    public int CustomerId { get; set; }
    
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;
}

public record OrderDetailViewModel(
    int SalesSoId,
    string SoNo,
    DateTime OrderDate,
    int CustomerId,
    string CustomerName,
    string? Address,
    decimal GrandTotal,
    List<OrderItemDetailViewModel> Items
);

public record OrderItemDetailViewModel(
    int SalesSoLitemId,
    string ItemName,
    int Quantity,
    decimal Price,
    decimal Total
);
